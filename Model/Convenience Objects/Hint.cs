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
    class Hint : GameObject
    {
        public Hint()
            : base()
        {
            ImageNames = new List<string>() { "hint" };
            Tangibility = Colors.NoColors;
            PlayerTangibility = Colors.NoColors;
            Visibility = Colors.NoColors;
            IsStatic = true;
            Scale = 0.4f;
        }
    }
}
