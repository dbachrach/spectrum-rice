using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Spectrum.View
{
    public delegate void ItemClickedHandler();

    class MenuItem
    {
        private string _text = "";
        private SpriteFont _font;
        private Vector2 _position = Vector2.Zero;
        private Color _baseColor;
        private Color _selectedColor;

        public bool Selected { get; set; }
        public string Name { get; set; }
        public event ItemClickedHandler Clicked;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Internal name of the menu</param>
        /// <param name="text">Text to be drawn to the screen</param>
        /// <param name="font">Font to draw the text in</param>
        /// <param name="position">Where to draw the text</param>
        /// <param name="baseColor">Default color of the text</param>
        /// <param name="selectedColor">Highlighted color of the text</param>
        /// <param name="selected">Whether or not the menu item is highlighted</param>
        public MenuItem(string name, string text, SpriteFont font, Vector2 position, Color baseColor, Color selectedColor, bool selected)
        {
            Name = name;
            _text = text;
            _font = font;
            _position = position;
            _baseColor = baseColor;
            _selectedColor = selectedColor;
            Selected = selected;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Selected)
            {
                spriteBatch.DrawString(_font, _text, _position, _selectedColor);
            }
            else
            {
                spriteBatch.DrawString(_font, _text, _position, _baseColor);
            }
        }

        public void Click()
        {
            if (Clicked != null)
            {
                Clicked();
            }
        }
    }
}
