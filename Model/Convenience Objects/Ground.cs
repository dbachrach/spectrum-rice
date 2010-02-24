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
    class Ground : GameObject
    {
        protected int _w;
        protected int _h;

		public Ground() : base() {
			Pickupable = false;
            ImageName = null;
            IsStatic = true;
            Visibility = Colors.AllColors;
            InitialFriction = .8f;
		}

        public Ground(int w, int h)
            : base()
        {
            _w = w;
            _h = h;
        }

        public override void LoadContent(ContentManager theContentManager, GraphicsDevice graphicsDevice)
        {
            Texture = new GameTexture(0.0f, 1.0f, .5f);
            Texture.Load(theContentManager, graphicsDevice, ImageName, FrameCount, FramesPerSec, _w, _h);
            Texture.Pause();

            LoadPhysicsBody(Texture.TextureSize(), true);
        }
    }
}
