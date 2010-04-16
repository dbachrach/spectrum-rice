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
        // standing still, 
        //private int[] animationFrameOffset = { 0, 1, 5, 9, 12 };
        //private int[] animationFrameCount = { 1, 4, 4, 3, 2,};
        
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

        private const float xboxConstant = 0.6f;
        private const float _speed = 80;
        private const float _hops = -900;
        private Vector2 moveLeft = new Vector2(-_speed, 0);
        private Vector2 moveRight = new Vector2(_speed, 0);
        private Vector2 moveLeftAir = new Vector2(-_speed / 10, 0);
        private Vector2 moveRightAir = new Vector2(_speed / 10, 0);

        private Vector2 moveLeftX = new Vector2(-_speed * xboxConstant, 0);
        private Vector2 moveRightX = new Vector2(_speed * xboxConstant, 0);
        private Vector2 moveLeftAirX = new Vector2(-_speed * xboxConstant / 10, 0);
        private Vector2 moveRightAirX = new Vector2(_speed * xboxConstant / 10, 0);

        private Vector2 jumpUp = new Vector2(0, _hops);
        private float superJumpFactor = 1.75f;

        private float velocityCap = 750;

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

        private static Vector2 OffscreenVector = new Vector2(-10000, -10000);

        public bool IsTouchingGround { get; set; }
        public bool IsTouchingSuperGround { get; set; }

        public Player()
            : base()
        {
            Id = "player";
            ImageNames = new List<string>() {"pl"};
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

            InitialFriction = 2.0f;
            InitialLinearDrag = 2.0f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Possession != null)
            {
                Possession.Draw(spriteBatch, new Vector2(this.body.position.X, this.body.position.Y));
            }
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime theGameTime)
        {
            UpdateMovement();
            UpdateJump();

            UpdateColor();
            UpdateXEvent(theGameTime);

            if (Possession != null)
            {
                Possession.DirectionFacing = this.DirectionFacing;
                Possession.Update(theGameTime);
            }

            // limit the user's movement a bit for the faster xbox frame rate
            velocityCap = Globals.IsUsingXboxController() ? 600 : 750; 
            if (body.LinearVelocity.X > velocityCap)
            {
                body.LinearVelocity.X = velocityCap;
            }
            else if (body.LinearVelocity.X < -velocityCap)
            {
                body.LinearVelocity.X = -velocityCap;
            }

            base.Update(theGameTime);
        }

        private void UpdateMovement()
        {
            Textures[0].Pause();
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
                    this.body.ApplyImpulse(Globals.IsUsingXboxController() ? ((IsTouchingGround) ? moveLeftX : moveLeftAirX)
                        : ((IsTouchingGround) ? moveLeft : moveLeftAir));
                    //this.geom.FrictionCoefficient = 0.5f;
                }

                d = Direction.Left;
            }
            else if (Globals.UserInputHold(Keys.D, Buttons.LeftThumbstickRight))
            {
                if (!BlockRight)
                {
                    /* Chooses a horizontal vector that is adjusted either for moving on the ground or in the air */
                    this.body.ApplyImpulse(Globals.IsUsingXboxController() ? ((IsTouchingGround) ? moveRightX : moveRightAirX)
    : ((IsTouchingGround) ? moveRight : moveRightAir));
                    
                }

                d = Direction.Right;
            }

            if (d != Direction.None)
            {
                DirectionFacing = d;

                if (!IsTouchingGround)
                {
                    Textures[0].Pause();
                }
                else
                {
                    State = PlayerState.Walking;
                    Textures[0].Play();
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
            /* Only allow jump when player is on the ground and not in All Colors Mode */
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

        private void UpdateXEvent(GameTime theGameTime)
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
                                e.Execute(Container.DeferFuture);
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
                // Applies vertical jump (larger if on a super jump)
                body.ApplyImpulse(IsTouchingSuperGround ? jumpUp * superJumpFactor : jumpUp);
            }
        }

        /* Notifications */

        protected override void DidCollideWithObject(GameObject obj, ref ContactList contactList, bool physicsCollision)
        {
            if (Leader == null && obj is Platform)
            {
                Leader = obj;
                this.geom.FrictionCoefficient = 0.0f;
            }

            if (obj.Pickupable || (obj.Events != null && obj.Events.Count > 0))
            {
                NearObject = obj;

                if (NearObject.Events != null)
                {
                    foreach (Event e in NearObject.Events)
                    {
                        if (e.Type == EventType.Collision && e.CollisionTarget == this)
                        {
                            e.Execute(Container.DeferFuture);
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
                normal.Normalize();
    
                double NECrossP = normal.X * NorthEast.Y - normal.Y * NorthEast.X;
                double NWCrossP = normal.X * NorthWest.Y - normal.Y * NorthWest.X;

                if (NECrossP >= 0 && NWCrossP <= 0)
                {
                    // the normals are pointing fairly vertically
                    this.DidHitGround(obj);
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

        protected void DidHitGround(GameObject obj)
        {
            IsTouchingGround = true;

            if (obj is HeavyBlock) // todo: generalize
            {
                IsTouchingSuperGround = true;
            }

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
            IsTouchingSuperGround = false;

            Leader = null;
        }


        private void Pickup(GameObject obj)
        {
            
            Possession = obj;

            // move the object offscreen
            Possession.body.Position = OffscreenVector;

            obj.Reap(false);
        }

        private void Drop()
        {
            if (Possession == null)
            {
                return;
            }

            //Container.RemoveFromSimulator(this);
            //this.LoadPhysicsBody(new Vector2(this.body.position.X, this.body.position.Y + Possession.Size.Y / 2.0f), new Vector2(Size.X, Size.Y), this.IsStatic);

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

            Vector2 newPosition = new Vector2((int)(this.body.Position.X + offset), (int)(this.body.Position.Y + myHeight / 2.0 - Possession.Size.Y / 2.0));
            bool shouldDrop = !Container.checkCollision(Possession, newPosition, Possession.Size);
            
            if (shouldDrop)
            {
                Possession.body.Position = newPosition;
                Possession.body.LinearVelocity = this.body.LinearVelocity;
                Container.DeferAddGameObject(Possession);

                Possession = null;
            }
        }

        protected override void DidSeparateWithObject(GameObject other)
        {
            if (other == Leader)
            {
                Console.WriteLine("Sep");
                Leader = null;
                this.geom.FrictionCoefficient = 2.0f;
            }
            base.DidSeparateWithObject(other);
        }

        public void WinLevel()
        {
            Container.Win();
        }
        public void LoseLevel()
        {
            // TODO: Show Death Page
            Container.LoseLevel();
        }
    }
}
