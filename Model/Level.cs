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

using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Debug;

namespace Spectrum.Model
{
    class Level
    {
        private GameTexture Background;

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
        public string BackgroundImageName { get; set; }
        public int BackgroundFrameCount { get; set; }
        public int BackgroundFramesPerSec { get; set; }

        // the color the user is viewing the level at the moment
        public Colors CurrentColor { get; set; }

        public Vector2 StartPosition { get; set; }

        // the colors this level can be viewed in
        public Colors AllowedColors { get; set; }

        // the objects contained in this level
        public List<GameObject> GameObjects { get; set; }

        public Vector2 TopCorner = new Vector2(0, 0);

        public Game1 GameRef { get; set; }

        public PhysicsSimulator Sim { get; set; }
        private PhysicsSimulatorView SimView;

        public bool DebugMode { get; set; }


        public Player player;

        private SpriteFont font;

        private List<GameObject> DoomedObjects;
        private List<GameObject> ResurrectedObjects;

        public int Gravity = 900;

		/* Default Constructor */
		public Level() {
			Completed = false;
			CurrentColor = Colors.NoColors;
			AllowedColors = Colors.AllColors;
            GameObjects = new List<GameObject>();

            DoomedObjects = new List<GameObject>();
            ResurrectedObjects = new List<GameObject>();

            BackgroundFrameCount = 1;
            BackgroundFramesPerSec = 1;

            Sim = new PhysicsSimulator(new Vector2(0, Gravity));
            //Sim.Iterations = 10; TODO: We can increase this value for better accuracy at the expense of performance
            
            SimView = new PhysicsSimulatorView(Sim);

            DebugMode = true;
		}

        public void AddGameObject(GameObject obj)
        {
            GameObjects.Add(obj);
        }
        public void DeferAddGameObject(GameObject obj)
        {
            ResurrectedObjects.Add(obj);
        }
        public void Obliterate(GameObject obj)
        {
            RemoveObjectFromLevel(obj);
            Sim.Remove(obj.body);
            Sim.Remove(obj.geom);
        }

        // Calls through to RemoveObjectFromLevel not Obliterate
        public void DeferObliterate(GameObject obj)
        {
            DoomedObjects.Add(obj);
            Sim.Remove(obj.body);
            Sim.Remove(obj.geom);
        }

        public void RemoveObjectFromLevel(GameObject obj)
        {
            GameObjects.Remove(obj);
        }
        public void DeferRemoveObjectFromLevel(GameObject obj) 
        {
            DoomedObjects.Add(obj);
        }

        /* TODO: Add remove methods that remove both object and remove body from physics engine */
        
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
            SimView.LoadContent(graphicsDevice, manager);

            if (BackgroundImageName != null && !BackgroundImageName.Equals(""))
            {
                Background = new GameTexture(0.0f, 1.0f, .5f);
                Background.Load(manager, graphicsDevice, BackgroundImageName, BackgroundFrameCount, BackgroundFramesPerSec);
                Background.Pause();
            }
            
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
                    RemoveObjectFromLevel(DoomedObj);
                }
                DoomedObjects.RemoveAll(item => true);
            }
            if (ResurrectedObjects != null)
            {
                foreach (GameObject ResurrectedObj in ResurrectedObjects)
                {
                    AddGameObject(ResurrectedObj);
                }
                ResurrectedObjects.RemoveAll(item => true);
            }

            player.ResetStatus();

            Sim.Update(gameTime.ElapsedGameTime.Milliseconds * .001f);

            foreach (GameObject obj in GameObjects)
            {
                obj.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Background != null)
            {
                Background.DrawFrame(spriteBatch, CurrentColor, Vector2.Zero, SpriteEffects.None);
            }

            foreach (GameObject obj in GameObjects)
            {
                obj.Draw(spriteBatch);
            }

            if (DebugMode)
            {
                SimView.Draw(spriteBatch);
            }
            /*
            if (CurrentColor.Equals(Colors.PurpleColor))
            {
                foreach (GameObject obj in GameObjects)
                {
                    if (obj.Visibility.Contains(Colors.RedColor))
                    {
                        Console.WriteLine("Found red object");
                        foreach (GameObject obj2 in GameObjects)
                        {
                            if (obj2.Visibility.Contains(Colors.BlueColor) && obj.Position().X == obj2.Position().X && obj.Position().Y == obj2.Position().Y)
                            {
                                Console.WriteLine("Found two on top ");
                                obj.Visibility.AddRawColors(RawColor.Purple);
                                obj.Draw(spriteBatch);
                            }
                        }
                    }
                }
            }
            */
            if (Completed)
            {
                string displayName = "Level Complete";
                spriteBatch.DrawString(font, displayName, new Vector2(350, 540), Color.White);
            }
            else if (player.NearObject != null)
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
                spriteBatch.DrawString(font, displayName, new Vector2(350, 540), Color.White);
            }

            string label = (player.BlockLeft ? "true" : "false") + " " + (player.BlockRight ? "true" : "false");
            spriteBatch.DrawString(font, label, new Vector2(350, 540), Color.White);


        }

        public override string ToString()
        {
            return string.Format("Level-- id {0}\nnumber {1}\n width {2}\n height {3}\n allowed colors {4}", this.Id, this.Number, this.Width, this.Height, this.AllowedColors);
        }

    }
}
