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

        private KeyboardState PreviousKeyboardState { get; set; }

        private PlayerState State { get; set; }

        private Vector2 StartingPosition { get; set; }

        public GameObject NearObject { get; set;}

        private const float _speed = 35;
        private const float _hops = -1000;
        private Vector2 moveLeft = new Vector2(-_speed, 0);
        private Vector2 moveRight = new Vector2(_speed, 0);
        private Vector2 moveLeftAir = new Vector2(-_speed / 2, 0);
        private Vector2 moveRightAir = new Vector2(_speed / 2, 0);
        private Vector2 jumpUp = new Vector2(0, _hops);

        private const int JumpAmount = 25;

        /* To Determine if he's standing on solid ground */
        private static float minAngle = -45;
        private static float minRad = (float)((-minAngle + 90.0) * (Math.PI / 180.0));
        private static Vector2 MinVector = new Vector2((float)Math.Cos(minRad), (float)Math.Sin(minRad));
        private static float maxAngle = 45;
        private static float maxRad = (float)((-maxAngle + 90.0) * (Math.PI / 180.0));
        private static Vector2 MaxVector = new Vector2((float)Math.Cos(maxRad), (float)Math.Sin(maxRad));

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
            KeyboardState aCurrentKeyboardState = Keyboard.GetState();


            UpdateMovement(aCurrentKeyboardState);
            UpdateJump(aCurrentKeyboardState);

            UpdateColor(aCurrentKeyboardState);
            UpdateXEvent(aCurrentKeyboardState);

            if (aCurrentKeyboardState.IsKeyDown(Keys.D) == true && PreviousKeyboardState.IsKeyDown(Keys.D) == false)
            {
                Container.DebugMode = !Container.DebugMode;
            }

            PreviousKeyboardState = aCurrentKeyboardState;

            if (Possession != null)
            {
                //Possession.body.Position = new Vector2(this.body.Position.X, this.body.Position.Y - Possession.geom.Height);
                // TODO: Move the possesion to above the player
                Possession.DirectionFacing = this.DirectionFacing;
                Possession.Update(theGameTime);

            }

            base.Update(theGameTime);
        }

        private void UpdateMovement(KeyboardState aCurrentKeyboardState)
        {
            Texture.Pause();
            Direction d = Direction.None;
            if (Keyboard.GetState().IsKeyDown(Keys.Left) == true)
            {
                /* Chooses a horizontal vector that is adjusted either for moving on the ground or in the air */
                this.body.ApplyImpulse((IsTouchingGround) ? moveLeft : moveLeftAir);

                d = Direction.Left;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right) == true)
            {
                /* Chooses a horizontal vector that is adjusted either for moving on the ground or in the air */
                this.body.ApplyImpulse((IsTouchingGround) ? moveRight : moveRightAir);

                d = Direction.Right;
            }

            if (d != Direction.None)
            {
                DirectionFacing = d;

                if (State == PlayerState.Jumping || State == PlayerState.Falling)
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

        private void UpdateJump(KeyboardState aCurrentKeyboardState)
        {
            if (State != PlayerState.Jumping)
            {
                if (aCurrentKeyboardState.IsKeyDown(Keys.Space) == true && PreviousKeyboardState.IsKeyDown(Keys.Space) == false)
                {
                    Jump();
                }
            }
        }

        private void UpdateColor(KeyboardState aCurrentKeyboardState)
        {
            if (aCurrentKeyboardState.IsKeyDown(Keys.A) == true && PreviousKeyboardState.IsKeyDown(Keys.A) == false)
            {
                Container.CurrentColor = Container.CurrentColor.ForwardColor();
            }
            else if (aCurrentKeyboardState.IsKeyDown(Keys.Z) == true && PreviousKeyboardState.IsKeyDown(Keys.Z) == false)
            {
                Container.CurrentColor = Container.CurrentColor.BackwardColor();
            }

            if (aCurrentKeyboardState.IsKeyDown(Keys.R) == true)
            {
                Container.CurrentColor = Colors.AllColors;
            }
            else if (aCurrentKeyboardState.IsKeyDown(Keys.R) == false && PreviousKeyboardState.IsKeyDown(Keys.R) == true)
            {
                Container.CurrentColor = Colors.RedColor;
            }

        }

        private void UpdateXEvent(KeyboardState aCurrentKeyboardState)
        {
            if (aCurrentKeyboardState.IsKeyDown(Keys.X) == true && PreviousKeyboardState.IsKeyDown(Keys.X) == false)
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
            /*if (State != PlayerState.Jumping)
            {
                State = PlayerState.Jumping;
                Vector2 v = Velocity;
                v.Y -= JumpAmount;
                Velocity = v;
            }*/
            if (IsTouchingGround)
            {
                State = PlayerState.Jumping;
                body.ApplyImpulse(jumpUp);
            }
        }

        /* Notifications */

        protected override void DidCollideWithObject(GameObject obj, ref ContactList contactList)
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
            Vector2 normal = Vector2.Zero;

            foreach (Contact contact in contactList)
            {
                normal += contact.Normal;
            }

            double minCrossP = normal.X * MinVector.Y - normal.Y * MinVector.X;
            double maxCrossP = normal.X * MaxVector.Y - normal.Y * MaxVector.X;

            if (minCrossP >= 0 && maxCrossP <= 0)
            {
                this.DidHitGround();
            }

            base.DidCollideWithObject(obj, ref contactList);
        }

        protected override void DidHitGround()
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
        }


        private void Pickup(GameObject obj)
        {
            Possession = obj;

            obj.body.position = new Vector2(this.body.position.X, this.body.position.Y - (this.Size.Y / 2) - (obj.Size.Y / 2));
            obj.body.Mass = 0.0001f;
            SliderJoint connector = JointFactory.Instance.CreateSliderJoint(this.body, new Vector2(0, -(this.Size.Y / 2)), obj.body, new Vector2(0, (obj.Size.Y / 2)), 0, 0);

            Container.Sim.Add(connector);

            Container.DeferRemoveGameObject(obj);
        }

        private void Drop()
        {
            /* TODO: Check to see if we are dropping on an existent object */

            /*if (Possession == null)
            {
                return;
            }

            int myWidth = (int) this.Size().X;
            int myHeight = (int)this.Size().Y;

            int offset = 0;
            if (DirectionFacing == Direction.Left)
            {
                offset = -myWidth; 
                //offset = -1;
            }
            else if (DirectionFacing == Direction.Right)
            {
                offset = myWidth;
                //offset = 1;
            }
            Possession.SetPosition( (int) (this.Position().X + offset), (int) (this.Position().Y + myHeight - Possesion.Size().Y  Possession.Position().Y));
            //Possession.Velocity = new Vector2(offset, Possession.Velocity.Y);
            Container.DeferAddGameObject(Possession);

            Possession = null;*/
        }

        protected override void DidLoadPhysicsBody()
        {
            geom.FrictionCoefficient = 2.0f;
            body.LinearDragCoefficient = 2.0f;
        }

        public void WinLevel()
        {
            Container.Completed = true;
        }
        public void LoseLevel()
        {

        }
    }
}
