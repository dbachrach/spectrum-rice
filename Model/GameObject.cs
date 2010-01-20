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

        public Rectangle Boundary { get { return _boundary; } set { _boundary = value; } }
        public string ImageName { get; set; }
        public bool AffectedByGravity { get; set; }		
        public Vector2 Velocity { get; set; }
        public List<GameObject> CombineObjects { get; set; }
		public List<GameObject> CombinableWith { get; set; }
        public bool Pickupable { get; set; }
        public bool Inactive { get; set; }
        public Texture2D InactiveImage { get; set; }
        public List<Event> Events { get; set; }
        public bool ExistsWhenNotViewed { get; set; }
        public Level Container { get; set; }
        public bool Animated { get; set; }
        public int FrameCount { get; set; }
        public int FramesPerSec { get; set; }
        public Direction DirectionFacing { get; set; }

        public Texture2D Texture { get; set; }
        public GameTexture AnimTexture; /* TODO: Rename Anim Texture */

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
            return new Vector2(_boundary.Left, _boundary.Right);
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
                //if (Animated)
                //{
                    AnimTexture.DrawFrame(spriteBatch, Container.CurrentColor, new Vector2(Boundary.Left, Boundary.Top), DrawEffects());
                //}
                //else
                //{
                //    spriteBatch.Draw(Texture, Position(), null, Container.CurrentColor.SystemColor() /*TODO: Should be Color.White when we have custom images for each color */, 0, Vector2.Zero, 1.0f, DrawEffects(), 0.0f);
                //}
                
            }
        }

        //Update the Sprite and change it's position based on the passed in speed, direction and elapsed time.
        public virtual void Update(GameTime theGameTime)
        {
            //Position += Velocity /** (float)theGameTime.ElapsedGameTime.TotalSeconds*/;

            SetPosition((int) (Position().X + Velocity.X), (int) (Position().Y + Velocity.Y));

            if (Animated)
            {
                float elapsed = (float)theGameTime.ElapsedGameTime.TotalSeconds;
                AnimTexture.UpdateFrame(elapsed);
            }
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
    }
}
