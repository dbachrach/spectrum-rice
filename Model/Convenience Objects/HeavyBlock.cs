using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spectrum.Model
{
    class HeavyBlock : Block
    {
        public HeavyBlock()
            : base()
        {
            ImageName = "heavyBox";
            Pickupable = false;
            Mass = 4000;
            SuperJumpable = true;
        }

        public override void LoadTextures()
        {
            base.LoadTextures();

            GameTexture t = new GameTexture(0.0f, this.Scale, .5f);
            t.Load(Container.GameRef.Content, Container.GameRef.GraphicsDevice, "arrow", FrameCount, FramesPerSec);
            t.Pause();

            Textures.Add(t);
        }
    }
}
