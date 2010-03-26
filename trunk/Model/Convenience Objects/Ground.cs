﻿using System;
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
        protected int alpha;

		public Ground() : base() {
			Pickupable = false;
            ImageNames = null;
            IsStatic = true;
            Visibility = Colors.AllColors;
            InitialFriction = .8f;
            alpha = 150;
            Size = new Vector2(_w, _h);
            Scale = 1;
		}

        public Ground(int w, int h)
            : this()
        {
            _w = w;
            _h = h;
        }
        public Ground(int w, int h, int a)
            : this(w,h)
        {
            alpha = a;
        }

        public override void LoadContent(ContentManager theContentManager, GraphicsDevice graphicsDevice)
        {
            // TODO: The ground shouldn't override all of this stuff
            GameTexture t = new GameTexture(0.0f, 1.0f, .5f);
            t.Load(theContentManager, graphicsDevice, "", FrameCount, FramesPerSec, _w, _h, alpha);
            t.Pause();

            Size = t.TextureSize() * Scale;

            LoadPhysicsBody(t.TextureSize(), true);

            Textures.Add(t);
        }
    }
}
