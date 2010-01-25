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
    class SolidGround : Ground
    {
        private int _w;
        private int _h;
		public SolidGround() : base() {
			ViewableColors = Colors.AllColors;
		}
        public SolidGround(int w, int h)
            : base()
        {
            ViewableColors = Colors.AllColors;
            _w = w;
            _h = h;
        }

        public override void LoadContent(ContentManager theContentManager, GraphicsDevice graphicsDevice)
        {
            Texture = new GameTexture(Vector2.Zero, 0.0f, 1.0f, .5f);
            Texture.Load(theContentManager, graphicsDevice, ImageName, FrameCount, FramesPerSec, _w, _h);
            Texture.Pause();

            LoadPhysicsBody(Texture.TextureSize(), true);
        }
    }
}
