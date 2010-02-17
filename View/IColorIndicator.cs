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
using Spectrum.Model;

namespace Spectrum.View
{
    interface IColorIndicator
    {
        void LoadContent(ContentManager manager, GraphicsDevice graphicsDevice);
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        void SetWheelColor(Colors colors);
        void DidChangeColor(Colors colors, bool clockwise);

        bool MoveBG { get; set; }
    }
}
