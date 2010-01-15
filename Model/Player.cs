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

        private KeyboardState PreviousKeyboardState { get; set; }


        public Player()
            : base()
        {
            Id = -1;
            ImageName = "dude";
            Position = new Vector2(50, 300);
            TimesDied = 0;
            PlayTime = TimeSpan.Zero;
            Possession = null;
        }

        public Player(double id, int timesDied, TimeSpan playTime, GameObject possession, Polygon polygon, string imageName)
            : base(id, Colors.NoColors, polygon, imageName, new Vector2(0,0), true, new Vector2(0,0), null, null, false, false, null, null, true, null)
        {
            TimesDied = timesDied;
            PlayTime = playTime;
            Possession = possession;
        }

        public void Update(GameTime theGameTime)
        {
            KeyboardState aCurrentKeyboardState = Keyboard.GetState();
            UpdateMovement(aCurrentKeyboardState);
            PreviousKeyboardState = aCurrentKeyboardState;
            base.Update(theGameTime);
        }

        private void UpdateMovement(KeyboardState aCurrentKeyboardState)
        {
            if (aCurrentKeyboardState.IsKeyDown(Keys.Left) == true)
            {
                Vector2 v = Velocity;
                v.X = -5;
            }
            else if (aCurrentKeyboardState.IsKeyDown(Keys.Right) == true)
            {
                Vector2 v = Velocity;
                v.X = 5;
            }
        }
    }
}
