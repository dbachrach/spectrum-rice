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
    enum PlayerState { None, Walking, Jumping }

    class Player : GameObject
    {
        public int TimesDied { get; set; }

        public TimeSpan PlayTime { get; set; }

        public GameObject Possession { get; set; }

        private KeyboardState PreviousKeyboardState { get; set; }

        private PlayerState State { get; set; }

        private Vector2 StartingPosition { get; set; }

        public GameObject NearObject { get; set;}

        private const float MaxJumpHeight = 200.0f;

        public Player()
            : base()
        {
            Id = "player";
            ImageName = "PlayerRun";
            TimesDied = 0;
            PlayTime = TimeSpan.Zero;
            Possession = null;
            State = PlayerState.Walking;
            Animated = true;
            FrameCount = 4;
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
            UpdateFall(aCurrentKeyboardState);

            UpdateColor(aCurrentKeyboardState);
            UpdateXEvent(aCurrentKeyboardState);
            PreviousKeyboardState = aCurrentKeyboardState;

            if (Possession != null)
            {
                Possession.Position = new Vector2(this.Position.X, this.Position.Y - Possession.Texture.Height); /* Change this from going through possesion's texture to going through polygon */
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




            if (Keyboard.GetState().IsKeyDown(Keys.Left) == true)
            {
                if (CollisionDetect(Direction.Left))
                {
                    return;
                }

                Vector2 v = Velocity;
                v.X = -3;
                Velocity = v;
                DirectionFacing = Direction.Left;

                if (State == PlayerState.Jumping)
                {
                    AnimTexture.Pause();
                }
                else
                {
                    State = PlayerState.Walking;
                    AnimTexture.Play();
                }
            }
            else if (aCurrentKeyboardState.IsKeyDown(Keys.Right) == true)
            {
                if (CollisionDetect(Direction.Right))
                {
                    return;
                }

                Vector2 v = Velocity;
                v.X = 3;
                Velocity = v;
                DirectionFacing = Direction.Right;

                if (State == PlayerState.Jumping)
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
            int moveY = 0 ;
            if (dir == Direction.Left)
            {
                moveX = -3;
            }
            else if (dir == Direction.Right)
            {
                moveX = 3;
            }
            else if (dir == Direction.Up)
            {
                moveY = -3;
            }
            else if (dir == Direction.Down)
            {
                moveY = 3;
            }


            Rectangle playerRectangle = new Rectangle((int)Position.X + moveX, (int)Position.Y + moveY, (int)AnimTexture.TextureSize().X, (int)AnimTexture.TextureSize().Y);
            foreach (GameObject obj in Container.GameObjects)
            {
                /* Check for collisions with player to an obj */
                if (obj != this)
                {
                    Rectangle objRectangle = new Rectangle((int)obj.Position.X, (int)obj.Position.Y, obj.Texture.Width, obj.Texture.Height);

                    if (playerRectangle.Intersects(objRectangle) && this.currentlyVisible() && obj.currentlyVisible())
                    {
                        NearObject = obj;
                        Console.WriteLine("Near object {0}", NearObject.Id);
                        /* TODO: This whole collision thing needs to be redone */

                        if (NearObject.Events != null)
                        {
                            Console.WriteLine("Has events");
                            foreach (Event e in NearObject.Events)
                            {
                                Console.WriteLine("Event e {0} {1} {2}", e, e.CollisionTarget,e.Type);
                                if (e.Type == EventType.Collision && e.CollisionTarget == this)
                                {
                                    Console.WriteLine("Collision with player event");
                                    e.Execute();
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

            if (State == PlayerState.Jumping)
            {
                /* Check to see if they've reached the apex of the jump */

                if (StartingPosition.Y - Position.Y > MaxJumpHeight || CollisionDetect(Direction.Up))
                {
                    Vector2 vel = Velocity;
                    vel.Y = 0;
                    Velocity = vel;
                    State = PlayerState.None;
                    return;
                }
            }
        }

        private void UpdateFall(KeyboardState aCurrentKeyboardState)
        {
            if (State != PlayerState.Jumping)
            {
                if (!CollisionDetect(Direction.Down))
                {
                    Vector2 v = Velocity;
                    v.Y = 3;
                    Velocity = v;
                }
                else
                {
                    Vector2 v = Velocity;
                    v.Y = 0;
                    Velocity = v;
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
                if (CollisionDetect(Direction.Up))
                {
                    return;
                }
                State = PlayerState.Jumping;
                StartingPosition = Position;
                Vector2 v = Velocity;
                v.Y = -3;
                Velocity = v;
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
            Possession.Position = new Vector2(this.Position.X + offset, this.Position.Y + myHeight - Possession.Texture.Height); // TODO: don't use texture
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
