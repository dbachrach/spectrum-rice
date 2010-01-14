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
    class Level
    {
        // a unique id for this level
        public int Id { get; set; }

        // the number of this level in the sequence of the levels
        public int Number { get; set; }

        // the name of the level
        public string Name { get; set; }

        // width of the level
        public int Width { get; set; }

        // height of the level
        public int Height { get; set; }

        // flag whether this level has been completed
        public bool Completed { get; set; }
        
        // the background image of this level
        public Texture2D Background { get; set; }

        // the color the user is viewing the level at the moment
        public Colors CurrentColor { get; set; }

        // the colors this level can be viewed in
        public Colors AllowedColors { get; set; }

        public Vector2 TopCorner = new Vector2(0, 0);

        public Level(int id, int number, string name, int width, int height, Colors allowedColors)
        {
            Id = id;
            Number = number;
            Name = name;
            Width = width;
            Height = height;
            AllowedColors = allowedColors;

            Completed = false;
            CurrentColor = Colors.NoColors;
        }

        // loads the background image from the specified file
        public void LoadContent(ContentManager manager, string backgroundFile)
        {
            Background = manager.Load<Texture2D>(backgroundFile);
        }

        // draws the background image
        public void Draw(SpriteBatch sprite)
        {
            sprite.Draw(Background, TopCorner, Color.White);
        }
    }
}
