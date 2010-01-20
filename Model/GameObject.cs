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
        public Polygon Polygon { get; set; }
        public string ImageName { get; set; }
        public Vector2 Position { get; set; }
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
        public AnimatedTexture AnimTexture;

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
		}

        public bool currentlyVisible()
        {
            return Container.CurrentColor.Contains(this.ViewableColors);
        }

        //Load the texture for the sprite using the Content Pipeline
        public void LoadContent(ContentManager theContentManager, GraphicsDevice graphicsDevice)
        {
            if (Animated)
            {
                AnimTexture = new AnimatedTexture(Vector2.Zero, 0.0f, 1.0f, .5f);
                AnimTexture.Load(theContentManager, ImageName, FrameCount, FramesPerSec);
                AnimTexture.Pause();
            }
            else if (ImageName == null || ImageName.Equals(""))
            {
                Texture = CreateRectangle(800, 120, graphicsDevice);
            }
            else
            {
                Texture = theContentManager.Load<Texture2D>(ImageName);
            }
        }

        //Draw the sprite to the screen
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(currentlyVisible()) {
                if (Animated)
                {
                    AnimTexture.DrawFrame(spriteBatch, Position, Container.CurrentColor.SystemColor(), DrawEffects());
                }
                else
                {
                    spriteBatch.Draw(Texture, Position, null, Container.CurrentColor.SystemColor() /*TODO: Should be Color.White when we have custom images for each color */, 0, Vector2.Zero, 1.0f, DrawEffects(), 0.0f);
                }
                
            }
        }

        //Update the Sprite and change it's position based on the passed in speed, direction and elapsed time.
        public virtual void Update(GameTime theGameTime)
        {
            Position += Velocity /** (float)theGameTime.ElapsedGameTime.TotalSeconds*/;

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

        private Texture2D CreateRectangle(int width, int height, GraphicsDevice graphicsDevice)
        {
            Texture2D rectangleTexture = new Texture2D(graphicsDevice, width, height, 1, TextureUsage.None, SurfaceFormat.Color);// create the rectangle texture, ,but it will have no color! lets fix that
            Color[] color = new Color[width * height];//set the color to the amount of pixels in the textures
            for (int i = 0; i < color.Length; i++)//loop through all the colors setting them to whatever values we want
            {
                color[i] = new Color(0,0,0,150);
            }
            rectangleTexture.SetData(color);//set the color data on the texture
            return rectangleTexture;//return the texture
        }

    }
}
