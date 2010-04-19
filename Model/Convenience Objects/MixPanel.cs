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
    class MixPanel : GameObject
    {
        public MixPanel()
            : base()
        {
            ImageNames = new List<string>() { "textBoxMix" };
            Tangibility = Colors.NoColors;
            PlayerTangibility = Colors.NoColors;
            Visibility = Colors.NoColors;
            IsStatic = true;
        }
    }
}
