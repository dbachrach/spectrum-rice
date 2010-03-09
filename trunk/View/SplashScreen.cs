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
    class SplashScreen : MenuScreen
    {

        public SplashScreen(SpectrumGame g, int width, int height)
            : base(g, width, height)
        {
            Image = "SplashScreen";
        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            base.LoadContent(content, graphics);
            Color baseColor = Color.Black;
            Color selectedColor = Color.Red;

            MenuItem m1 = new MenuItem(Globals.NewGameMenuItem, Globals.NewGameMenuItem, font, new Vector2(550f, 200f), baseColor, selectedColor, false);
            m1.Clicked += delegate()
            {
                game.StartGame();
                game.Paused = false;
                selectedItem = 0;
            };
            menuItems.Add(m1);

            MenuItem m2 = new MenuItem(Globals.ContinueGameMenuItem, Globals.ContinueGameMenuItem, font, new Vector2(550f, 250f), baseColor, selectedColor, false);
            m2.Clicked += delegate()
            {
                game.StartGame(); // TODO: Load level index
                game.Paused = false;
                selectedItem = 0;
            };
            menuItems.Add(m2);

            MenuItem m3 = new MenuItem(Globals.CreditsMenuItem, Globals.CreditsMenuItem, font, new Vector2(550f, 300f), baseColor, selectedColor, false);
            m3.Clicked += delegate()
            {
                // TODO: Show Credits
            };
            menuItems.Add(m3);
        }
    }
}
