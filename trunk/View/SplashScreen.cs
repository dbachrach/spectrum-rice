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
    class SplashScreen
    {

        private SpriteFont font;
        private MenuItem[] menuItem = new MenuItem[3];
        private int selectedItem = 0;
        private GameTexture background;
        private SpectrumGame game;

        private int ViewWidth;
        private int ViewHeight;

        public SplashScreen(SpectrumGame g, int width, int height)
        {
            background = new GameTexture(0, 1.0f, 1.0f);
            background.AssetCount = 1;
            game = g;

            ViewWidth = width;
            ViewHeight = height;
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            font = content.Load<SpriteFont>("Pesca");

            Color baseColor = Color.Black;
            Color selectedColor = Color.Red;

            menuItem[0] = new MenuItem("New Game", "New Game", font, new Vector2(550f, 200f), baseColor, selectedColor, false);
            menuItem[1] = new MenuItem("Continue Game", "Continue Game", font, new Vector2(550f, 250f), baseColor, selectedColor, false);
            menuItem[2] = new MenuItem("Credits", "Credits", font, new Vector2(550f, 300f), baseColor, selectedColor, false);

            background.Load(content, graphics, "SplashScreen", 1, 1, ViewWidth, ViewHeight);
            background.Pause();
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
                if (menuItem[selectedItem].Name.Equals("New Game"))
                {
                    game.StartGame();
                    game.Paused = false;
                    selectedItem = 0;
                }
                else if (menuItem[selectedItem].Name.Equals("Continue Game"))
                {
                    game.StartGame(); // TODO: Load level index
                    game.Paused = false;
                    selectedItem = 0;
                }
                else if (menuItem[selectedItem].Name.Equals("Credits"))
                {
                    // TODO: Show credits
                }
                
            }

            menuItem[selectedItem].Selected = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            background.DrawFrame(spriteBatch, Colors.AllColors, new Vector2(ViewWidth / 2, ViewHeight / 2), SpriteEffects.None, false);
            for (int i = 0; i < menuItem.Length; i++)
            {
                menuItem[i].Draw(spriteBatch);
            }
        }
    }
}
