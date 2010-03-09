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
    class Platform : GameObject
    {
        private int _width;
		public Platform(int w) : base() {

            ImageName = "plat";
            IsStatic = true;
            if (w == 0)
            {
                Scale = 1.0f;
            }
            else
            {
                Scale = 0.25f;
            }

            InitialFriction = 0.0f;
            Mass = 10000;
            _width = w;
		}

        public override void LoadTexture()
        {
            if (_width == 0)
            {
                base.LoadTexture();
                return;
            }

            GraphicsDevice g = Container.GameRef.GraphicsDevice;

            const int leftWidth = 79;
            const int rightWidth = 78;
            const int minWidth = leftWidth + rightWidth;
            const int centerWidth = 150;
            const int allHeight = 1221;

            int w = Math.Max((int) (_width * (1 / Scale)), minWidth);
            int repeats = (w - minWidth) / centerWidth;
            if (w > minWidth)
            {
                w = minWidth + repeats * centerWidth;
            }

            RenderTarget2D targ = new RenderTarget2D(g, w, allHeight, 1, SurfaceFormat.Rgba1010102);
            DepthStencilBuffer stenBuf = Globals.CreateDepthStencil(targ);
            // Cache the current depth buffer
            DepthStencilBuffer oldStenBuf = g.DepthStencilBuffer;
            

            g.SetRenderTarget(0, targ);
            // Set our custom depth buffer
            g.DepthStencilBuffer = stenBuf;
            g.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.TransparentWhite, 1.0f, 0); 

            SpriteBatch b = new SpriteBatch(g);
            b.Begin();
            Texture2D left = Container.GameRef.Content.Load<Texture2D>("platformEndLeft");
            b.Draw(left, new Vector2(0, 0), Color.White);
            Texture2D right = Container.GameRef.Content.Load<Texture2D>("platformEndRight");
            b.Draw(right, new Vector2(w - rightWidth, 0), Color.White);
            Texture2D mid = Container.GameRef.Content.Load<Texture2D>("platformCenter");
            for (int cnt = 0; cnt < repeats; cnt++)
            {
                b.Draw(mid, new Vector2(leftWidth + (centerWidth * cnt), 0), Color.White);
            }
            b.End();

            
            g.SetRenderTarget(0, null);
            g.DepthStencilBuffer = oldStenBuf;

            Texture2D platformTexture = targ.GetTexture();

            // TODO: I have no idea why this works
            b.Begin();
            b.Draw(platformTexture, Vector2.Zero, Color.White);
            b.End();

            Texture = new GameTexture(0.0f, this.Scale, .5f);
            Texture.Load(platformTexture, FrameCount, FramesPerSec);
            Texture.Pause();
            Texture.AssetCount = 7;

            Size = Texture.TextureSize() * this.Scale;
        }
            
    }
}
