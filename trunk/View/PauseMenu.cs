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
    class PauseMenu : MenuScreen
    {
        
        public PauseMenu(SpectrumGame g, int width, int height)
            : base(g, width, height)
        {
            
        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            base.LoadContent(content, graphics);
            Color baseColor = Color.White;
            Color selectedColor = Color.Red;

            MenuItem m1 = new MenuItem(Globals.ResumeMenuItem, Globals.ResumeMenuItem, font, new Vector2(350f, 150f), baseColor, selectedColor, false);
            m1.Clicked += delegate()
            {
                game.Paused = false;
                selectedItem = 0;
            };
            menuItems.Add(m1);

            MenuItem m2 = new MenuItem(Globals.RestartMenuItem, Globals.RestartMenuItem, font, new Vector2(350f, 200f), baseColor, selectedColor, false);
            m2.Clicked += delegate()
            {
                game.Restart();
                game.Paused = false;
                selectedItem = 0;
            };
            menuItems.Add(m2);

            MenuItem m3 = new MenuItem(Globals.SettingsMenuItem, Globals.SettingsMenuItem, font, new Vector2(350f, 250f), baseColor, selectedColor, false);
            m3.Clicked += delegate()
            {
                // TODO: Show settings
            };
            menuItems.Add(m3);

            int i = 0;
            foreach (string lev in game.levelsPresentable)
            {
                int x = i;
                MenuItem m = new MenuItem("Level " + (i + 1) + ": " + lev, "Level " + (i + 1) + ": " + lev, font, new Vector2(350f, 300f + 50f * i), baseColor, selectedColor, false);
                m.Clicked += delegate()
                {
                    Console.WriteLine("load lev " + x);
                    game.Paused = false;
                    game.LoadLevel(x, true);
                    selectedItem = 0;
                };
                menuItems.Add(m);
                i++;
            }

            

            MenuItem m4 = new MenuItem(Globals.ExitMenuItem, Globals.ExitMenuItem, font, new Vector2(350f, 300f +50f*i), baseColor, selectedColor, false);
            m4.Clicked += delegate()
            {
                game.Exit();
                selectedItem = 0;
            };
            menuItems.Add(m4);
        }
    }
}
