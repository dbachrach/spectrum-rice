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
    class MenuScreen
    {
        protected SpriteFont font;
        protected List<MenuItem> menuItems;
        protected int selectedItem;
        protected GameTexture background;
        protected SpectrumGame game;
        public string Image { get; set; }

        protected int ViewWidth;
        protected int ViewHeight;

        public MenuScreen(SpectrumGame g, int width, int height)
        {
            menuItems = new List<MenuItem>();
            selectedItem = 0;
            background = new GameTexture(0, 1.0f, 1.0f);
            background.AssetCount = 1;
            game = g;
            ViewWidth = width;
            ViewHeight = height;
            Image = null;
        }

        public virtual void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            font = content.Load<SpriteFont>("mvboli");
            background.Load(content, graphics, Image, 1, 1, ViewWidth, ViewHeight, 185);
            background.Pause();
        }

        public void Update(GameTime gameTime)
        {
            foreach (MenuItem m in menuItems)
            {
                m.Selected = false;
            }

            if (Globals.UserInputPress(Keys.Up, Buttons.LeftThumbstickUp))
            {
                selectedItem = (selectedItem + menuItems.Count() - 1) % menuItems.Count;
            }

            else if (Globals.UserInputPress(Keys.Down, Buttons.LeftThumbstickDown))
            {
                selectedItem = (selectedItem + 1) % menuItems.Count;
            }

            else if (Globals.UserInputPress(Keys.Enter, Buttons.A))
            {
                menuItems[selectedItem].Click();
            }

            menuItems[selectedItem].Selected = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            background.DrawFrame(spriteBatch, Colors.AllColors, new Vector2(ViewWidth / 2, ViewHeight / 2), SpriteEffects.None, false);
            foreach (MenuItem m in menuItems)
            {
                m.Draw(spriteBatch);
            }
        }
    }
}
