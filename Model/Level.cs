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
        public string Id { get; set; }

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

        public Game1 GameRef { get; set; }

        private Player player;

        private SpriteFont font;

        private List<GameObject> DoomedObjects;
        private List<GameObject> ResurrectedObjects;

        public int Gravity = 1;

		/* Default Constructor */
		public Level() {
			Completed = false;
			CurrentColor = Colors.NoColors;
			AllowedColors = Colors.AllColors;
            GameObjects = new List<GameObject>();

            DoomedObjects = new List<GameObject>();
            ResurrectedObjects = new List<GameObject>();
		}

        public void AddGameObject(GameObject obj)
        {
            GameObjects.Add(obj);
        }
        public void DeferAddGameObject(GameObject obj)
        {
            ResurrectedObjects.Add(obj);
        }
        public void RemoveGameObject(GameObject obj)
        {
            GameObjects.Remove(obj);
        }
        public void DeferRemoveGameObject(GameObject obj)
        {
            DoomedObjects.Add(obj);
        }
        
        public void AddPlayer(Player p)
        {
            player = p;
            AddGameObject(p);
        }

        public GameObject GameObjectForId(string i)
        {
            foreach (GameObject o in GameObjects) 
            {
                if (o.Id.Equals(i))
                {
                    return o;
                }
            }
            return null;
        }

        public void LoadContent(ContentManager manager, GraphicsDevice graphicsDevice)
        {
            //Background = manager.Load<Texture2D>("sunset");
            font = manager.Load<SpriteFont>("Pesca");

            foreach (GameObject obj in GameObjects)
            {
                obj.LoadContent(manager, graphicsDevice);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (DoomedObjects != null)
            {
                foreach (GameObject DoomedObj in DoomedObjects)
                {
                    RemoveGameObject(DoomedObj);
                }
            }
            if (ResurrectedObjects != null)
            {
                foreach (GameObject ResurrectedObj in ResurrectedObjects)
                {
                    AddGameObject(ResurrectedObj);
                }
            }
            foreach (GameObject obj in GameObjects)
            {
                obj.Update(gameTime);
            }          
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(Background, TopCorner, Color.White);

            foreach (GameObject obj in GameObjects)
            {
                obj.Draw(spriteBatch);
            }
            /*
            if (CurrentColor.Equals(Colors.PurpleColor))
            {
                foreach (GameObject obj in GameObjects)
                {
                    if (obj.ViewableColors.Contains(Colors.RedColor))
                    {
                        Console.WriteLine("Found red object");
                        foreach (GameObject obj2 in GameObjects)
                        {
                            if (obj2.ViewableColors.Contains(Colors.BlueColor) && obj.Position().X == obj2.Position().X && obj.Position().Y == obj2.Position().Y)
                            {
                                Console.WriteLine("Found two on top ");
                                obj.ViewableColors.AddRawColors(RawColor.Purple);
                                obj.Draw(spriteBatch);
                            }
                        }
                    }
                }
            }
            */
            if (player.NearObject != null)
            {
                string displayName = "";
                if (player.NearObject.Pickupable)
                {
                    displayName = "Pickup";
                }
                if (player.NearObject.Events != null)
                {
                    foreach (Event e in player.NearObject.Events)
                    {
                        if (e.Type == EventType.XEvent)
                        {
                            displayName = e.DisplayName;
                            break;
                        }
                    }
                }
                if (Completed)
                {
                    displayName =  "Level Complete";
                }
                spriteBatch.DrawString(font, displayName, new Vector2(350, 540), Color.White);
            }
        }

        public override string ToString()
        {
            return string.Format("Level-- id {0}\nnumber {1}\n width {2}\n height {3}\n allowed colors {4}", this.Id, this.Number, this.Width, this.Height, this.AllowedColors);
        }

    }
}
