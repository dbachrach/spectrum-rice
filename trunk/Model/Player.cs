using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Debug;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using FarseerGames.FarseerPhysics.Factories;

namespace Spectrum.Model
{
    enum PlayerState { None, Walking, Jumping, Falling }

    class Player : GameObject
    {
        public int TimesDied { get; set; }

        public TimeSpan PlayTime { get; set; }

        public GameObject Possession { get; set; }

        private PlayerState State { get; set; }

        private Vector2 StartingPosition { get; set; }

        public GameObject NearObject { get; set;}

        public Joint Connector { get; set; }

        // whether or not to allow the player to add sideways impulse to the player by pushing the
        // arrows keys
        public bool BlockLeft { get; set; }
        public bool BlockRight { get; set; }

        private const float _speed = 50;
        private const float _hops = -900;
        private Vector2 moveLeft = new Vector2(-_speed, 0);
        private Vector2 moveRight = new Vector2(_speed, 0);
        private Vector2 moveLeftAir = new Vector2(-_speed / 3, 0);
        private Vector2 moveRightAir = new Vector2(_speed / 3, 0);
        private Vector2 jumpUp = new Vector2(0, _hops);

        private const int JumpAmount = 25;

        /* To determine if he's standing on solid ground */
        private static float minAngle = -45;
        private static float minRad = (float)((-minAngle + 90.0) * (Math.PI / 180.0));
        private static Vector2 NorthEast = new Vector2((float)Math.Cos(minRad), (float)Math.Sin(minRad));
        private static float maxAngle = 45;
        private static float maxRad = (float)((-maxAngle + 90.0) * (Math.PI / 180.0));
        private static Vector2 NorthWest = new Vector2((float)Math.Cos(maxRad), (float)Math.Sin(maxRad));

        private static Vector2 SouthWest = -NorthEast;
        private static Vector2 SouthEast = -NorthWest;

        public bool IsTouchingGround { get; set; }

        public Player()
            : base()
        {
            Id = "player";
            ImageName = "pl";
            TimesDied = 0;
            PlayTime = TimeSpan.Zero;
            Possession = null;
            State = PlayerState.Walking;
            FrameCount = 5;
            FramesPerSec = 8;
            State = PlayerState.None;
            NearObject = null;
            BlockLeft = false;
            BlockRight = false;

            ZIndex = Globals.PlayerZIndex;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Possession != null)
            {
                Possession.Draw(spriteBatch);
            }

            base.Draw(spriteBatch);
        }

        public override void Update(GameTime theGameTime)
        {

            UpdateMovement();
            UpdateJump();

            UpdateColor();
            UpdateXEvent();


            if (Possession != null)
            {
                //Possession.body.Position = new Vector2(this.body.Position.X, this.body.Position.Y - Possession.geom.Height);
                // TODO: Move the possesion to above the player
                Possession.DirectionFacing = this.DirectionFacing;
                Possession.Update(theGameTime);

            }

            base.Update(theGameTime);
        }

        private void UpdateMovement()
        {
            Texture.Pause();
            Direction d = Direction.None;

            if (Container.allColorsMode())
            {
                /* If we are in ALL COLORS mode, then don't allow movement */
            }
            else if (Globals.UserInputHold(Keys.A, Buttons.LeftThumbstickLeft))
            {
                if (!BlockLeft)
                {
                    /* Chooses a horizontal vector that is adjusted either for moving on the ground or in the air */
                    this.body.ApplyImpulse((IsTouchingGround) ? moveLeft : moveLeftAir);
                }

                d = Direction.Left;
            }
            else if (Globals.UserInputHold(Keys.D, Buttons.LeftThumbstickRight))
            {
                if (!BlockRight)
                {
                    /* Chooses a horizontal vector that is adjusted either for moving on the ground or in the air */
                    this.body.ApplyImpulse((IsTouchingGround) ? moveRight : moveRightAir);
                }

                d = Direction.Right;
            }

            if (d != Direction.None)
            {
                DirectionFacing = d;

                if (!IsTouchingGround)
                {
                    Texture.Pause();
                }
                else
                {
                    State = PlayerState.Walking;
                    Texture.Play();
                }
            }
            else
            {
                //this.body.LinearVelocity = new Vector2(this.body.LinearVelocity.X / 10, this.body.LinearVelocity.Y);
                this.body.ApplyImpulse(new Vector2(-this.body.LinearVelocity.X / 4, 0));
            }
        }

        private void UpdateJump()
        {
            if (IsTouchingGround && !Container.allColorsMode())
            {
                if (Globals.UserInputPress(Keys.W, Buttons.A))
                {
                    Jump();
                }
            }
        }

        private void UpdateColor()
        {
            if (!Container.allColorsMode())
            {
                if (Globals.UserInputPress(Keys.Left, Buttons.LeftTrigger))
                {
                    Container.BackwardColor();
                }
                if (Globals.UserInputPress(Keys.Right, Buttons.RightTrigger))
                {
                    Container.ForwardColor();
                }
            }

            if (Globals.UserInputHold(Keys.Up, Buttons.RightShoulder))
            {
                Container.ActivateAllColorsMode();
            }
            else if (Globals.UserInputRelease(Keys.Up, Buttons.RightShoulder))
            {
                Container.DeactivateAllColorsMode();
            }

        }

        private void UpdateXEvent()
        {
            if (Globals.UserInputPress(Keys.E, Buttons.X) && !Container.allColorsMode())
            {
                if (Possession != null)
                {
                    Drop();
                }
                else if (NearObject != null && NearObject.Pickupable)
                {
                    Pickup(NearObject);
                }
                else if (NearObject != null) 
                {
                    if (NearObject.Events != null)
                    {
                        foreach (Event e in NearObject.Events)
                        {
                            if (e.Type == EventType.XEvent)
                            {
                                e.Execute();
                            }
                        }
                    }
                }

            }
        }

        private void Jump()
        {
            if (IsTouchingGround)
            {
                State = PlayerState.Jumping;
                body.ApplyImpulse(jumpUp);
            }
        }

        /* Notifications */

        protected override void DidCollideWithObject(GameObject obj, ref ContactList contactList, bool physicsCollision)
        {
            /* TODO: Need to unset NearObject when not near anything */
            if (obj.Pickupable || (obj.Events != null && obj.Events.Count > 0))
            {
                NearObject = obj;

                if (NearObject.Events != null)
                {
                    foreach (Event e in NearObject.Events)
                    {
                        if (e.Type == EventType.Collision && e.CollisionTarget == this)
                        {
                            e.Execute();
                        }
                    }
                }
            }

            // check if the player is touching the ground
            // if his normal vector is pointing upwards (plus or minus 45 degrees), he is on solid ground
            if (physicsCollision)
            {
                Vector2 normal = Vector2.Zero;

                foreach (Contact contact in contactList)
                {
                    normal += contact.Normal;
                }

                double NECrossP = normal.X * NorthEast.Y - normal.Y * NorthEast.X;
                double NWCrossP = normal.X * NorthWest.Y - normal.Y * NorthWest.X;

                if (NECrossP >= 0 && NWCrossP <= 0)
                {
                    // the normals are pointing fairly vertically
                    this.DidHitGround();
                }
                else
                {
                    double SWCrossP = normal.X * SouthWest.Y - normal.Y * SouthWest.X;
                    double SECrossP = normal.X * SouthEast.Y - normal.Y * SouthEast.X;

                    if (SECrossP >= 0 && NECrossP <= 0)
                    {
                        // the normals are pointing to the right
                        // that means there is something immediately to our left
                        // block the player from moving there
                        BlockLeft = true;
                    }
                    else if (NWCrossP >= 0 && SWCrossP <= 0)
                    {
                        // the normals are pointing to the left
                        // that means there is something immediately to our right
                        // block the player from moving there
                        BlockRight = true;
                    }
                }
            }

            base.DidCollideWithObject(obj, ref contactList, physicsCollision);
        }

        protected void DidHitGround()
        {
            IsTouchingGround = true;
            // todo: do we want to remove this and whole idea of state
            if (State == PlayerState.Jumping)
            {
                State = PlayerState.None;
            }
        }

        // before each simulation step, reset the player's status variables
        public void ResetStatus()
        {
            IsTouchingGround = false;
            NearObject = null;
            BlockLeft = false;
            BlockRight = false;
        }


        private void Pickup(GameObject obj)
        {
            Possession = obj;

            obj.body.position = new Vector2(this.body.position.X, this.body.position.Y - (this.Size.Y / 2) - (obj.Size.Y / 2));
            obj.body.Mass = 0.0001f;
            SliderJoint connector = JointFactory.Instance.CreateSliderJoint(this.body, new Vector2(0, -(this.Size.Y / 2)), obj.body, new Vector2(0, (obj.Size.Y / 2)), 0, 0);

            Connector = connector;
            Container.Sim.Add(connector);

            obj.Reap(false);
        }

        private void Drop()
        {
            /* TODO: Check to see if we are dropping on an existent object */

            if (Possession == null)
            {
                return;
            }

            int myWidth = (int)this.Size.X;
            int myHeight = (int)this.Size.Y;

            int offset = 0;
            if (DirectionFacing == Direction.Left)
            {
                offset = -myWidth;
            }
            else if (DirectionFacing == Direction.Right)
            {
                offset = myWidth;
            }
            Possession.body.Position = new Vector2( (int) (this.body.Position.X + offset), (int) (this.body.Position.Y + myHeight/2.0 - Possession.Size.Y/2.0));
            Container.DeferAddGameObject(Possession);

            // Restores mass from what it used to be before we zeroed it
            Possession.body.Mass = Possession.Mass;

            // remove the joint from the simulator
            Container.Sim.Remove(Connector);
            Connector = null;

            Possession = null;
        }

        protected override void DidLoadPhysicsBody()
        {
            geom.FrictionCoefficient = 2.0f;
            body.LinearDragCoefficient = 2.0f;
        }

        public void WinLevel()
        {
            Container.Win();
        }
        public void LoseLevel()
        {
            // TODO: Show Death Page
            Container.Restart();
        }
    }
}
