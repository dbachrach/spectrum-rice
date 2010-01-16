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
        public double Id { get; set; }

        // the number of this level in the sequence of the levels
        public double Number { get; set; }

        // the name of the level
        public string Name { get; set; }

        // width of the level
        public double Width { get; set; }

        // height of the level
        public double Height { get; set; }

        // flag whether this level has been completed
        public bool Completed { get; set; }
        
        // the background image of this level
        public Texture2D Background { get; set; }

        // the color the user is viewing the level at the moment
        public Colors CurrentColor { get; set; }

        public Vector2 StartPosition { get; set; }

        // the colors this level can be viewed in
        public Colors AllowedColors { get; set; }

        // the objects contained in this level
        public List<GameObject> GameObjects { get; set; }

        public Vector2 TopCorner = new Vector2(0, 0);

        

		/* Default Constructor */
		public Level() {
			Completed = false;
			CurrentColor = Colors.NoColors;
			AllowedColors = Colors.AllColors;
            GameObjects = new List<GameObject>();
		}
        public Level(double id, double number, string name, double width, double height, Colors allowedColors)
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

        public void AddGameObject(GameObject obj)
        {
            GameObjects.Add(obj);
        }

        // loads the background image from the specified file
        public void LoadContent(ContentManager manager)
        {
            //Background = manager.Load<Texture2D>("sunset");

            foreach (GameObject obj in GameObjects)
            {
                obj.LoadContent(manager);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (GameObject obj in GameObjects)
            {
                obj.Update(gameTime);
            }
        }

        // draws the background image
        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(Background, TopCorner, Color.White);

            foreach (GameObject obj in GameObjects)
            {
                obj.Draw(spriteBatch);
            }
        }

        public override string ToString()
        {
            return string.Format("Level-- id {0}\nnumber {1}\n width {2}\n height {3}\n allowed colors {4}", this.Id, this.Number, this.Width, this.Height, this.AllowedColors);
        }

    }
}
