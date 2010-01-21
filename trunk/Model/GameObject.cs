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
    enum Direction { Up, Down, Left, Right, None }

    class GameObject
    {
        public string Id { get; set; }
        public Colors ViewableColors { get; set; }

        private Rectangle _boundary;
        private Vector2 _velocity;

        public Rectangle Boundary { get { return _boundary; } set { _boundary = value; } }
        public string ImageName { get; set; }
        public bool AffectedByGravity { get; set; }
        public Vector2 Velocity { get { return _velocity; } set { _velocity = value; } }
        public List<GameObject> CombineObjects { get; set; }
		public List<GameObject> CombinableWith { get; set; }
        public bool Pickupable { get; set; }
        public bool Inactive { get; set; }
        public Texture2D InactiveImage { get; set; }
        public List<Event> Events { get; set; }
        public bool ExistsWhenNotViewed { get; set; }
        public Level Container { get; set; }
        public bool Animated { get; set; } /* TODO: Get rid of this property */
        public int FrameCount { get; set; }
        public int FramesPerSec { get; set; }
        public Direction DirectionFacing { get; set; }

        public Texture2D Texture { get; set; }
        public GameTexture AnimTexture; /* TODO: Rename Anim Texture */

        private float GravityConstant = 1.3f;

		/* Default Constructor */
		public GameObject() {
			ViewableColors = Colors.AllColors;
			AffectedByGravity = true;
			Velocity = new Vector2(0,0);
            CombineObjects = null;
			CombinableWith = null;
			Pickupable = false;
			Inactive = false;
			ExistsWhenNotViewed = true;
            Animated = false;
            DirectionFacing = Direction.Right;

            FrameCount = 1;
            FramesPerSec = 1;
		}

        public void SetPosition(int x, int y)
        {
            _boundary.Location = new Point(x, y);
        }

        public void SetSize(int width, int height)
        {
            _boundary.Width = width;
            _boundary.Height = height;
        }
        public Vector2 Position()
        {
            return new Vector2(_boundary.Left, _boundary.Top);
        }
        public Vector2 Size()
        {
            return new Vector2(_boundary.Width, _boundary.Height);
        }

        public bool currentlyVisible()
        {
            return Container.CurrentColor.Contains(this.ViewableColors);
        }

        //Load the texture for the sprite using the Content Pipeline
        public void LoadContent(ContentManager theContentManager, GraphicsDevice graphicsDevice)
        {
            
            AnimTexture = new GameTexture(Vector2.Zero, 0.0f, 1.0f, .5f);
            AnimTexture.Load(theContentManager, graphicsDevice, ImageName, FrameCount, FramesPerSec);
            AnimTexture.Pause();

            SetSize((int)AnimTexture.TextureSize().X, (int)AnimTexture.TextureSize().Y);
        }

        //Draw the sprite to the screen
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(currentlyVisible()) {
                AnimTexture.DrawFrame(spriteBatch, Container.CurrentColor, new Vector2(Boundary.Left, Boundary.Top), DrawEffects());
            }
        }

        //Update the Sprite and change it's position based on the passed in speed, direction and elapsed time.
        public virtual void Update(GameTime theGameTime)
        {

            if (AffectedByGravity && currentlyVisible())
            {
                Velocity = new Vector2(Velocity.X, Velocity.Y + GravityConstant);
            }

            if (!Velocity.Equals(Vector2.Zero) && !CheckCollision())
            {
                SetPosition((int)(Position().X + Velocity.X), (int)(Position().Y + Velocity.Y));
            }

            if (Animated)
            {
                float elapsed = (float)theGameTime.ElapsedGameTime.TotalSeconds;
                AnimTexture.UpdateFrame(elapsed);
            }
        }

        // If object collides, then the position & velocity of the object is updated
        private bool CheckCollision()
        {
            
            Rectangle bothRect = new Rectangle((int)(Position().X + Velocity.X), (int)(Position().Y + Velocity.Y), (int)Size().X, (int)Size().Y);
            GameObject obj = CollisionWithRect(bothRect);

            // see if the object collides with anything
            if (obj == null)
            {
                return false;
            }

            Rectangle yRect = new Rectangle((int)(Position().X), (int)(Position().Y + Velocity.Y), (int)Size().X, (int)Size().Y);
            if (obj.Boundary.Intersects(yRect))
            {
                // if falling
                // TODO: what happens if it's zero?
                if (Velocity.Y == 0)
                {
                    // do nothing
                }
                else if (Velocity.Y > 0)
                {
                    SetPosition((int)(Position().X), (int)(obj.Position().Y - Size().Y));
                    this.DidHitGround();
                }
                else // if jumping
                {
                    SetPosition((int)(Position().X), (int)(obj.Position().Y + obj.Size().Y));
                }
                
                Velocity = new Vector2(Velocity.X, 0);
            }

            Rectangle xRect = new Rectangle((int)(Position().X + Velocity.X), (int)(Position().Y), (int)Size().X, (int)Size().Y);
            if (obj.Boundary.Intersects(xRect))
            {
                // if moving right
                // TODO: what happens if it's zero?
                if (Velocity.X == 0)
                {

                }
                else if (Velocity.X > 0)
                {
                    SetPosition((int)(obj.Position().X - Size().X), (int)(Position().Y));
                }
                else // if moving left
                {
                    SetPosition((int)(obj.Position().X + obj.Size().X), (int)(Position().Y));
                }

                Velocity = new Vector2(0, Velocity.Y);
            }

            return true;
        }

        private GameObject CollisionWithRect(Rectangle rect)
        {
            foreach (GameObject obj in Container.GameObjects)
            {
                /* Check for collisions with player to an obj */
                if (obj != this && this.currentlyVisible() && obj.currentlyVisible())
                {
                    if (rect.Intersects(obj.Boundary))
                    {
                        return obj;
                    }
                }
            }
            return null;
        }

        // Returns a SpriteEffects value based on the DirectionFacing property of this GameObject
        public SpriteEffects DrawEffects()
        {
            SpriteEffects effects = SpriteEffects.None;

            if (DirectionFacing == Direction.Left)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            return effects;
        }

        protected virtual void DidHitGround()
        {
            // nada
        }
    }
}
