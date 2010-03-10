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

using Spectrum.View;

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

        // position where the player starts at
        public Vector2 StartPosition { get; set; }

        // the colors this level can be viewed in
        public Colors AllowedColors { get; set; }

        // indicates which color the level starts in
        public Colors StartingColor { get; set; }

        // the objects contained in this level
        public List<GameObject> GameObjects { get; set; }

        public Vector2 TopCorner = new Vector2(0, 0);

        public SpectrumGame GameRef { get; set; }

        public PhysicsSimulator Sim { get; set; }
        private PhysicsSimulatorView SimView;

        public bool DebugMode { get; set; }

        // if the player travels outside these bounds, he will be killed
        private const int KILL_VAL = 10;

        public Player player;

        public SpriteFont font;

        private List<GameObject> DoomedObjects;
        private List<GameObject> ResurrectedObjects;

        private IColorIndicator colorIndicator;

        public int Gravity = 3000;

        private bool _allColorsMode;

        public bool allColorsMode()
        {
            return _allColorsMode;
        }

        private bool useColorBar = true;

        public List<EventAction> FutureActions;
        public List<EventAction> DeferFuture;

        public Vector2 CameraPosition { get; set; }

        public GameTime CurrentTime { get; set; }

		/* Default Constructor */
		public Level() {
            
            if (useColorBar)
            {
                colorIndicator = new ColorBar();
            }
            else
            {
                colorIndicator = new ColorWheel();
            }
			Completed = false;
			CurrentColor = Colors.NoColors;
			AllowedColors = Colors.AllColors;

            GameObjects = new List<GameObject>();

            DoomedObjects = new List<GameObject>();
            ResurrectedObjects = new List<GameObject>();

            BackgroundFrameCount = 1;
            BackgroundFramesPerSec = 1;

            Sim = new PhysicsSimulator(new Vector2(0, Gravity));
            Sim.FrictionType = FrictionType.Minimum;
            //Sim.Iterations = 10; TODO: We can increase this value for better accuracy at the expense of performance
            
            SimView = new PhysicsSimulatorView(Sim);

            DebugMode = false;

            _allColorsMode = false;

            FutureActions = new List<EventAction>();
            DeferFuture = new List<EventAction>();

            CameraPosition = Vector2.Zero;
		}

        // Adds obj to the level
        public void AddGameObject(GameObject obj)
        {
            GameObjects.Add(obj);
        }

        // Adds obj to the level at the next update cycle.
        public void DeferAddGameObject(GameObject obj)
        {
            ResurrectedObjects.Add(obj);
        }

        // Removes obj from the level and from the simulator.
        public void Obliterate(GameObject obj)
        {
            RemoveObjectFromLevel(obj);
            Sim.Remove(obj.body);
            Sim.Remove(obj.geom);
        }

        // Removes obj from the level and from the simulator at the next update cycle.
        // (Calls through to RemoveObjectFromLevel not Obliterate)
        public void DeferObliterate(GameObject obj)
        {
            DoomedObjects.Add(obj);
            Sim.Remove(obj.body);
            Sim.Remove(obj.geom);
        }

        // Immediately removes obj from the level
        public void RemoveObjectFromLevel(GameObject obj)
        {
            GameObjects.Remove(obj);
        }

        // Removes obj from the level at the next update cycle.
        public void DeferRemoveObjectFromLevel(GameObject obj) 
        {
            DoomedObjects.Add(obj);
        }
        
        // Adds p to the level and marks p as the player of the level
        public void AddPlayer(Player p)
        {
            player = p;
            AddGameObject(p);
        }

        public void RemoveFromSimulator(GameObject obj)
        {
            Sim.Remove(obj.body);
            Sim.Remove(obj.geom);
        }

        // Returns a game object within this level with the id i. 
        // Throws an exception if object was not found.
        public GameObject GameObjectForId(string i)
        {
            foreach (GameObject o in GameObjects) 
            {
                if (o.Id.Equals(i))
                {
                    return o;
                }
            }

            throw new Exception("Could not find object in level with id " + i);
        }


        public void Win()
        {
            Completed = true;
            GameRef.Win();
        }
        public void Restart()
        {
            GameRef.Restart();
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

            colorIndicator.LoadContent(manager, graphicsDevice);
            colorIndicator.SetColor(StartingColor);
        }

        public void Update(GameTime gameTime)
        {
            CurrentTime = gameTime;

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

            
            double ms = gameTime.TotalRealTime.TotalMilliseconds;
            
            foreach (EventAction a in FutureActions)
            {
                if (ms >= a.LaunchTime)
                {
                    a.Execute(DeferFuture, ms);
                }
            }
            FutureActions.RemoveAll(item => ms >= item.LaunchTime);
            FutureActions.AddRange(DeferFuture);
            DeferFuture.RemoveAll(item => true);
            
            foreach (GameObject obj in GameObjects)
            {
                obj.Update(gameTime);
            }

            colorIndicator.Update(gameTime);

            checkPlayerDeath();

            AdjustCamera();

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Background != null)
            {
                Background.DrawFrame(spriteBatch, CurrentColor, Vector2.Zero, SpriteEffects.None, false);
            }

            foreach (GameObject obj in GameObjects)
            {
                obj.Draw(spriteBatch);
            }

            if (DebugMode)
            {
                SimView.Draw(spriteBatch, CameraPosition);
            }

            if (Completed)
            {
                string displayName = "Level Complete";
                spriteBatch.DrawString(font, displayName, new Vector2(350, (int)this.Height-50), Color.White);
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

                spriteBatch.DrawString(font, displayName, new Vector2(350, (int)this.Height - 50), Color.White);
            }

            /*if (player.BlockLeft)
            {
                string label = "block left";
                spriteBatch.DrawString(font, label, new Vector2(450, (int)this.Height - 50), Color.White);
            }

            if (player.BlockRight)
            {
                string label = "block right";
                spriteBatch.DrawString(font, label, new Vector2(550, (int)this.Height - 50), Color.White);
            }*/

            colorIndicator.Draw(spriteBatch);
        }

        public override string ToString()
        {
            return string.Format("Level-- id {0}\nnumber {1}\n width {2}\n height {3}\n allowed colors {4}", this.Id, this.Number, this.Width, this.Height, this.AllowedColors);
        }

        public void ForwardColor()
        {
            if (colorIndicator.MoveBG)
            {
                do
                {
                    CurrentColor = CurrentColor.BackwardColor();
                } while (!AllowedColors.Contains(CurrentColor));

                colorIndicator.DidChangeColor(CurrentColor, false);
            }
            else
            {
                do
                {
                    CurrentColor = CurrentColor.ForwardColor();
                } while (!AllowedColors.Contains(CurrentColor));

                colorIndicator.DidChangeColor(CurrentColor, true);
            }
        }

        public void BackwardColor()
        {
            if (colorIndicator.MoveBG)
            {
                do
                {
                    CurrentColor = CurrentColor.ForwardColor();
                } while (!AllowedColors.Contains(CurrentColor));

                colorIndicator.DidChangeColor(CurrentColor, true);
            }
            else
            {
                do
                {
                    CurrentColor = CurrentColor.BackwardColor();
                } while (!AllowedColors.Contains(CurrentColor));

                colorIndicator.DidChangeColor(CurrentColor, false);
            }
        }

        public void ActivateAllColorsMode()
        {
            //PreviousColor = CurrentColor;
            //CurrentColor = Colors.AllColors;
            _allColorsMode = true;
        }

        // Must be called only once after a call to ActivateAllColorsMode()
        public void DeactivateAllColorsMode()
        {
            //CurrentColor = PreviousColor;
            //PreviousColor = Colors.NoColors;
            _allColorsMode = false;
        }

        private void checkPlayerDeath()
        {
            float x = player.body.Position.X;
            float y = player.body.Position.Y;

            if (x < -KILL_VAL || x > Width + KILL_VAL || y < -KILL_VAL || y > Height + KILL_VAL)
            {
                Restart();
            }
        }

        public void AdjustCamera()
        {
            float x = (float)Math.Min(Math.Max(player.body.position.X - Globals.GameWidth / 2, 0), Width - Globals.GameWidth);
            float y = (float)Math.Min(Math.Max(player.body.position.Y - Globals.GameHeight / 2, 0), Height - Globals.GameHeight);
            CameraPosition = new Vector2(x, y);
        }
    }
}
