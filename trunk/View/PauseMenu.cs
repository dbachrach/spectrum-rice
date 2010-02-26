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
    class PauseMenu
    {
        private SpriteFont font;
        private MenuItem[] menuItem = new MenuItem[7];
        private int selectedItem = 0;
        private GameTexture pauseBackground;
        private Game1 game;

        private int ViewWidth;
        private int ViewHeight;

        public PauseMenu(Game1 g, int width, int height)
        {
            pauseBackground = new GameTexture(0, 1.0f, 1.0f);
            game = g;

            ViewWidth = width;
            ViewHeight = height;
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            font = content.Load<SpriteFont>("Pesca");

            Color baseColor = Color.White;
            Color selectedColor = Color.Red;

            menuItem[0] = new MenuItem(Globals.ResumeMenuItem, Globals.ResumeMenuItem, font, new Vector2(350f, 150f), baseColor, selectedColor, false);
            menuItem[1] = new MenuItem(Globals.RestartMenuItem, Globals.RestartMenuItem, font, new Vector2(350f, 200f), baseColor, selectedColor, false);
            menuItem[2] = new MenuItem(Globals.SettingsMenuItem, Globals.SettingsMenuItem, font, new Vector2(350f, 250f), baseColor, selectedColor, false);
            menuItem[3] = new MenuItem("lev1", "Level 1: Training Day", font, new Vector2(350f, 300f), baseColor, selectedColor, false);
            menuItem[4] = new MenuItem("lev2", "Level 2: Dr. Evil's Quarters", font, new Vector2(350f, 350f), baseColor, selectedColor, false);
            menuItem[5] = new MenuItem("lev3", "Level 3: Combination Pizza Hut", font, new Vector2(350f, 400f), baseColor, selectedColor, false);
            menuItem[6] = new MenuItem(Globals.ExitMenuItem, Globals.ExitMenuItem, font, new Vector2(350f, 450f), baseColor, selectedColor, false);

            pauseBackground.Load(content, graphics, null, 1, 1, ViewWidth, ViewHeight);
            pauseBackground.Pause();
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < menuItem.Length; i++)
            {
                menuItem[i].Selected = false;
            }

            if (Globals.UserInputPress(Keys.Up, Buttons.LeftThumbstickUp))
            {
                selectedItem -= 1;
                if (selectedItem == -1)
                {
                    selectedItem = menuItem.Length - 1;
                }
            }

            if (Globals.UserInputPress(Keys.Down, Buttons.LeftThumbstickDown))
            {
                selectedItem += 1;
                if (selectedItem == menuItem.Length)
                {
                    selectedItem = 0;
                }
            }

            if (Globals.UserInputPress(Keys.Enter, Buttons.A))
            {
                if (menuItem[selectedItem].Name.Equals(Globals.ResumeMenuItem))
                {
                    game.Paused = false;
                    selectedItem = 0;
                }
                else if (menuItem[selectedItem].Name.Equals(Globals.RestartMenuItem))
                {
                    game.Restart();
                    game.Paused = false;
                    selectedItem = 0;
                }
                else if (menuItem[selectedItem].Name.Equals(Globals.SettingsMenuItem))
                {
                    // TODO: Show settings
                }
                else if (menuItem[selectedItem].Name.Equals(Globals.ExitMenuItem))
                {
                    game.Exit();
                    selectedItem = 0;
                }
                else if (menuItem[selectedItem].Name.Equals("lev1"))
                {
                    game.Paused = false;
                    game.LoadLevel(0,true);

                    selectedItem = 0;
                }
                else if (menuItem[selectedItem].Name.Equals("lev2"))
                {
                    game.Paused = false;
                    game.LoadLevel(1,true);
                    selectedItem = 0;
                }
                else if (menuItem[selectedItem].Name.Equals("lev3"))
                {
                    game.Paused = false;
                    game.LoadLevel(2,true);
                    selectedItem = 0;
                }
            }

            menuItem[selectedItem].Selected = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            pauseBackground.DrawFrame(spriteBatch, Colors.AllColors, new Vector2(ViewWidth/2, ViewHeight/2), SpriteEffects.None, false);
            for (int i = 0; i < menuItem.Length; i++)
            {
                menuItem[i].Draw(spriteBatch);
            }
        }
    }
}
