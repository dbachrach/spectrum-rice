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
    class Player : GameObject
    {
        public int TimesDied { get; set; }

        public TimeSpan PlayTime { get; set; }

        public GameObject Possession { get; set; }

        public Player(int id, int timesDied, TimeSpan playTime, GameObject possession, Polygon polygon, Texture2D image)
            : base(id, Colors.NoColors, polygon, image, new Vector2(0,0), true, new Vector2(0,0), null, null, false, false, null, null, true, null)
        {
            TimesDied = timesDied;
            PlayTime = playTime;
            Possession = possession;
        }
    }
}
