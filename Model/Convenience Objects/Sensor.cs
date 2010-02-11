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

namespace Spectrum.Model.Convenience_Objects
{
    // TODO: Sensor repeats its event calls
    class Sensor : GameObject
    {
        protected int _w;
        protected int _h;

        public bool OneTime { get; set; }

        public Sensor()
            : base()
        {
            IsSensor = true;
            PlayerTangibility = Colors.AllColors;
            Tangibility = Colors.AllColors;
            Visibility = Colors.NoColors;
            IsStatic = true; // Sensors are static by default
        }

        public Sensor(int w, int h)
            : this()
        {
            _w = w;
            _h = h;
        }

        public override void LoadContent(ContentManager theContentManager, GraphicsDevice graphicsDevice)
        {
            Size = new Vector2(_w, _h);

            LoadPhysicsBody(Size, IsStatic);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            /* Sensors do not draw themselves */
        }


    }
}
