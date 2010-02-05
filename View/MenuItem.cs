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
    class MenuItem
    {
        private string _text = "";
        private SpriteFont _font;
        private Vector2 _position = Vector2.Zero;
        private Color _baseColor;
        private Color _selectedColor;

        public bool Selected { get; set; }
        public string Name { get; set; }

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
    }
}
