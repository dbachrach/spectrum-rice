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

        private const int MoveAmount = 2;
        private const int JumpAmount = 30;

        public Player()
            : base()
        {
            Id = "player";
            ImageName = "pl";
            TimesDied = 0;
            PlayTime = TimeSpan.Zero;
            Possession = null;
            State = PlayerState.Walking;
            Animated = true;
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
            //UpdateFall(aCurrentKeyboardState);

            UpdateColor(aCurrentKeyboardState);
            UpdateXEvent(aCurrentKeyboardState);

            PreviousKeyboardState = aCurrentKeyboardState;

            if (Possession != null)
            {
                Possession.SetPosition(this.Boundary.Left, this.Boundary.Top - Possession.Boundary.Height);
                //Possession.Position = new Vector2(this.Position.X, this.Position.Y - Possession.Texture.Height); /* Change this from going through possesion's texture to going through polygon */
                Possession.DirectionFacing = this.DirectionFacing;
                Possession.Update(theGameTime);

            }

            base.Update(theGameTime);
        }

        private void UpdateMovement(KeyboardState aCurrentKeyboardState)
        {
            Vector2 v1 = Velocity;
            v1.X = 0;
            Velocity = v1;
            AnimTexture.Pause();

            Direction d = Direction.None;

            if (Keyboard.GetState().IsKeyDown(Keys.Left) == true) 
            {
                d = Direction.Left;
            }
            else if (aCurrentKeyboardState.IsKeyDown(Keys.Right) == true) {
                d = Direction.Right;
            }

            if (d != Direction.None)
            {
                //if (CollisionDetect(d))
                //{
                //    return;
                //}

                Vector2 v = Velocity;
                if (d == Direction.Left)
                {
                    v.X = -MoveAmount;
                }
                else if (d == Direction.Right)
                {
                    v.X = MoveAmount;
                }
                
                Velocity = v;
                DirectionFacing = d;

                if (State == PlayerState.Jumping || State == PlayerState.Falling)
                {
                    AnimTexture.Pause();
                }
                else
                {
                    State = PlayerState.Walking;
                    AnimTexture.Play();
                }
            }
        }

        private bool CollisionDetect(Direction dir)
        {
            /* TODO: Modify this colision stuff to use polygons */

            /* TODO: Do we need to check moves 1, 2, and 3. not just 3. In the case that its a very thin wall */
            int moveX = 0;
            int moveY = 0;
            if (dir == Direction.Left)
            {
                moveX = -MoveAmount;
            }
            else if (dir == Direction.Right)
            {
                moveX = MoveAmount;
            }
            else if (dir == Direction.Up)
            {
                moveY = -MoveAmount;
            }
            else if (dir == Direction.Down)
            {
                moveY = MoveAmount;
            }


            Rectangle playerRectangle;
            if (Possession == null)
            {
                playerRectangle = new Rectangle( (int) (Position().X + moveX), (int) (Position().Y + moveY), (int) Size().X, (int) Size().Y);
            }
            else
            {
                playerRectangle = new Rectangle((int)(Position().X + moveX), (int)(Position().Y + moveY - Possession.Boundary.Height), (int)Size().X, (int)Size().Y + Possession.Boundary.Height);
            }
            
            foreach (GameObject obj in Container.GameObjects)
            {
                /* Check for collisions with player to an obj */
                if (obj != this && obj != Possession)
                {
                    Rectangle objRectangle = obj.Boundary;
                    if (playerRectangle.Intersects(objRectangle) && this.currentlyVisible() && obj.currentlyVisible())
                    {
                        if (obj.Pickupable || (obj.Events != null && obj.Events.Count > 0))
                        {
                            NearObject = obj;
                            Console.WriteLine("Near object {0}", NearObject.Id);
                            /* TODO: This whole collision thing needs to be redone */

                            if (NearObject.Events != null)
                            {
                                Console.WriteLine("Has events");
                                foreach (Event e in NearObject.Events)
                                {
                                    Console.WriteLine("Event e {0} {1} {2}", e, e.CollisionTarget, e.Type);
                                    if (e.Type == EventType.Collision && e.CollisionTarget == this)
                                    {
                                        Console.WriteLine("Collision with player event");
                                        e.Execute();
                                    }
                                }
                            }
                        }

                        return true;
                    }
                }
            }
            NearObject = null;
            return false;
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
            if (State != PlayerState.Jumping)
            {
                State = PlayerState.Jumping;
                Vector2 v = Velocity;
                v.Y -= JumpAmount;
                Velocity = v;
            }
        }

        protected override void DidHitGround()
        {
            if (State == PlayerState.Jumping)
            {
                State = PlayerState.None;
            }
        }

        private void Pickup(GameObject obj)
        {
            Possession = obj;
            Container.DeferRemoveGameObject(obj);
        }

        private void Drop()
        {
            /* TODO: Check to see if we are dropping on an existent object */

            if (Possession == null)
            {
                return;
            }

            int myWidth = (int) this.AnimTexture.TextureSize().X; // TODO: don't use texture;
            int myHeight = (int)this.AnimTexture.TextureSize().Y; // TODO: don't use texture;

            int offset = 0;
            if (DirectionFacing == Direction.Left)
            {
                offset = -myWidth; 
            }
            else if (DirectionFacing == Direction.Right)
            {
                offset = myWidth;
            }
            //Possession.Position = new Vector2(this.Position().X + offset, this.Position().Y + myHeight - Possession.Texture.Height); // TODO: don't use texture
            Possession.SetPosition( (int) (this.Position().X + offset), (int) (this.Position().Y + myHeight - Possession.Size().Y));
            Container.DeferAddGameObject(Possession);



            Possession = null;
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
