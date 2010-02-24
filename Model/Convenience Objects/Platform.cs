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
    class Platform : GameObject
    {
        private int _width;
		public Platform(int w) : base() {
            
            ImageName = "plat";
            IsStatic = true;
            Scale = .25f;

            InitialFriction = .6f;
            _width = w;
		}

        public override void LoadTexture()
        {
            GraphicsDevice g = Container.GameRef.GraphicsDevice;


            int w = Math.Max((int) (_width * (1 / Scale)), 264);
            int repeats = (w - 264) / 419;
            if (w > 264)
            {
                w = 264 + repeats * 419;
            }

            RenderTarget2D targ = new RenderTarget2D(g, w, 1275, 1, SurfaceFormat.Color, g.PresentationParameters.MultiSampleType, 0);
            DepthStencilBuffer stenBuf = Globals.CreateDepthStencil(targ);
            // Cache the current depth buffer
            DepthStencilBuffer oldStenBuf = g.DepthStencilBuffer;
            // Set our custom depth buffer
            g.DepthStencilBuffer = stenBuf;

            g.SetRenderTarget(0, targ);
            SpriteBatch b = new SpriteBatch(g);
            b.Begin();
            Texture2D left = Container.GameRef.Content.Load<Texture2D>("platformEndLeft");
            b.Draw(left, new Vector2(0, 0), Color.White);
            Texture2D right = Container.GameRef.Content.Load<Texture2D>("platformEndRight");
            b.Draw(right, new Vector2(w - 150, 0), Color.White);
            Texture2D mid = Container.GameRef.Content.Load<Texture2D>("platformCenter");
            for (int cnt = 0; cnt < repeats; cnt++)
            {
                b.Draw(mid, new Vector2(114 + (419 * cnt), 0), Color.White);
            }
            b.End();

            g.SetRenderTarget(0, null);
            g.DepthStencilBuffer = oldStenBuf;

            Texture2D platformTexture = targ.GetTexture();

            Texture = new GameTexture(0.0f, this.Scale, .5f);
            Texture.Load(platformTexture, FrameCount, FramesPerSec);
            Texture.Pause();

            Size = Texture.TextureSize() * this.Scale;
        }
            
    }
}
