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
    class GameObject
    {
        public double Id { get; set; }
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
        public SpriteEffects DrawEffects { get; set; }
        public bool Animated { get; set; }
        public int FrameCount { get; set; }
        public int FramesPerSec { get; set; }

        private Texture2D Texture { get; set; }
        private AnimatedTexture AnimTexture;

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
            DrawEffects = SpriteEffects.None;
            Animated = false;
		}

        public bool currentlyVisible()
        {
            return Container.CurrentColor.contains(this.ViewableColors);
        }

        //Load the texture for the sprite using the Content Pipeline
        public void LoadContent(ContentManager theContentManager)
        {
            if (Animated)
            {
                AnimTexture = new AnimatedTexture(Vector2.Zero, 0.0f, 1.0f, .5f);
                AnimTexture.Load(theContentManager, ImageName, FrameCount, FramesPerSec);
            }
            else
            {
                Texture = theContentManager.Load<Texture2D>(ImageName);
            }
        }

        //Draw the sprite to the screen
        public void Draw(SpriteBatch spriteBatch)
        {
            if(currentlyVisible()) {
                if (Animated)
                {
                    AnimTexture.DrawFrame(spriteBatch, Position, DrawEffects);
                }
                else
                {
                    spriteBatch.Draw(Texture, Position, null, Container.CurrentColor.SystemColor() /*TODO: Should be Color.White when we have custom images for each color */, 0, Vector2.Zero, 1.0f, DrawEffects, 0.0f);
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


    }
}
