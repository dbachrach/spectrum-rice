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
        private List<GameTexture> Background;

        /// <summary>
        /// Uniqute identifier for this level
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Number of this level in the sequence of levels
        /// </summary>
        public double Number { get; set; }

        /// <summary>
        /// Displayed name of the level
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Width in pixels of the level. Sizes greater than 1280 will cause the level
        /// to autmoatically scroll. 
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Height in pixels of the level. Sizes greater than 720 will cause the level
        /// to automatically scroll.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Whether this level has been completed by the player
        /// </summary>
        public bool Completed { get; set; }
        
        /// <summary>
        /// Name of the image file for the background of this image. Do not include file extension.
        /// By default this value is "levelBG", which will load the default background image.
        /// Since background images are very large, each color should be placed in its own file.
        /// Name each file with the same name followed by the color for that image.
        /// For example, the blue default image is named "levelBGblue", and the red is named "levelBGred".
        /// Animated background images are allowed. See <see cref="BackgroundFrameCount"/> and <see cref="BackgroundFrameCount"/>.
        /// </summary>
        public string BackgroundImageName { get; set; }
        public int BackgroundFrameCount { get; set; }
        public int BackgroundFramesPerSec { get; set; }

        /// <summary>
        /// Color player is currently viewing the level in
        /// </summary>
        public Colors CurrentColor { get; set; }

        /// <summary>
        /// Position where player starts when the level is loaded
        /// </summary>
        public Vector2 StartPosition { get; set; }

        /// <summary>
        /// Colors this level can be viewed in
        /// </summary>
        public Colors AllowedColors { get; set; }

        // indicates which color the level starts in
        public Colors StartingColor { get; set; }

        /// <summary>
        /// Allo objects contained in this level
        /// </summary>
        public List<GameObject> GameObjects { get; set; }

        public Vector2 TopCorner = new Vector2(0, 0);

        public SpectrumGame GameRef { get; set; }

        /// <summary>
        /// Physics simulator for this level
        /// </summary>
        public PhysicsSimulator Sim { get; set; }

        /// <summary>
        /// Specailzied view of the physics simulator
        /// </summary>
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
        /// <summary>
        /// Creates a default level. Also creates the physics simulator for the level.
        /// </summary>
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

            BackgroundImageName = "levelBG";
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

        /// <summary>
        /// Adds an object to the list of objects this level contains
        /// </summary>
        public void AddGameObject(GameObject obj)
        {
            GameObjects.Add(obj);
        }

        /// <summary>
        /// Adds an object to the list of objects this level contains, but does so at the next iteration of the update() loop.
        /// Useful adding game objects to a level while iterating through the level's objects.
        /// </summary>
        public void DeferAddGameObject(GameObject obj)
        {
            ResurrectedObjects.Add(obj);
        }

        /// <summary>
        /// Removes an object from the level and from the simulator
        /// </summary>
        public void Obliterate(GameObject obj)
        {
            RemoveObjectFromLevel(obj);
            Sim.Remove(obj.body);
            Sim.Remove(obj.geom);
        }

        /// <summary>
        /// Removes an object from the level and from the simulator, but does so at the next iteration of the update() loop.
        /// Useful for removing objects from a level while iterating through the level's objects.
        /// (Calls through to RemoveObjectFromLevel not Obliterate)
        /// </summary>
        public void DeferObliterate(GameObject obj)
        {
            DoomedObjects.Add(obj);
            Sim.Remove(obj.body);
            Sim.Remove(obj.geom);
        }

        /// <summary>
        /// Immediately removes obj from 
        /// </summary>
        public void RemoveObjectFromLevel(GameObject obj)
        {
            GameObjects.Remove(obj);
        }

        // 
        /// <summary>
        /// Removes obj from the level's list of game objects, but does so at the next iteration of the update() loop.
        /// Useful for removing objects from a level while iterating through the level's objects.
        /// </summary>
        public void DeferRemoveObjectFromLevel(GameObject obj) 
        {
            DoomedObjects.Add(obj);
        }
        
        /// <summary>
        /// Adds a player to the level's list of game objects and sets this player as the player of the level
        /// </summary>
        public void AddPlayer(Player p)
        {
            player = p;
            AddGameObject(p);
        }

        /// <summary>
        /// Removes an object from the physics simulator.
        /// Does not remove the object from the list of objects contained in the level.
        /// </summary>
        public void RemoveFromSimulator(GameObject obj)
        {
            Sim.Remove(obj.body);
            Sim.Remove(obj.geom);
        }

        /// <summary>
        /// Returns a game object within this level with a given id.
        /// Throws an exception if the object was not found.
        /// </summary>
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

        /// <summary>
        /// Wins the current level
        /// </summary>
        public void Win()
        {
            Completed = true;
            GameRef.Win();
        }

        /// <summary>
        /// Restarts the current level
        /// </summary>
        public void Restart()
        {
            GameRef.Restart();
        }

        public void LoadContent(ContentManager manager, GraphicsDevice graphicsDevice)
        {
            SimView.LoadContent(graphicsDevice, manager);

            if (BackgroundImageName != null && !BackgroundImageName.Equals(""))
            {
                Background = new List<GameTexture>();
                Console.WriteLine("bg image name " + BackgroundImageName);
                foreach (string color in Colors.ListOfColorsNames)
                {
                    GameTexture b;
                    b = new GameTexture(0.0f, 1.0f, .5f);
                    b.AssetCount = 1;
                    b.Load(manager, graphicsDevice, BackgroundImageName + color, BackgroundFrameCount, BackgroundFramesPerSec);
                    b.Pause();

                    Background.Add(b);
                }
            }
            
            font = manager.Load<SpriteFont>("Pesca");

            foreach (GameObject obj in GameObjects)
            {
                obj.LoadContent(manager, graphicsDevice);
            }

            
            colorIndicator.LoadContent(manager, graphicsDevice);
            colorIndicator.SetVisibleColors(AllowedColors);
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

            List<EventAction> toDelete = new List<EventAction>();

            foreach (EventAction a in FutureActions)
            {
                if (a.TicksRemaining == 0)
                {
                    toDelete.Add(a);
                    a.Execute(DeferFuture);
                }
                else
                {
                    a.TicksRemaining--;
                }
            }

            FutureActions.RemoveAll(item => toDelete.Contains(item));
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
                int index = CurrentColor.IndexIn(Colors.AllColors);
                Background[index].DrawFrame(spriteBatch, CurrentColor, new Vector2(Background[index].TextureSize().X / 2, Background[index].TextureSize().Y / 2), SpriteEffects.None, false);
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
                if (player.NearObject.Pickupable && player.Possession == null)
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

        public bool checkCollision(GameObject target, Vector2 pos, Vector2 size)
        {
            foreach (GameObject obj in GameObjects)
            {   
                if(obj != this.player && target.sharesAColorWith(obj) && RectangleIntersect(pos, size, obj.body.Position, obj.Size))
                {
                    return true;
                }
            }
            return false;
        }


        public static bool RectangleIntersect(Vector2 pos1, Vector2 size1, Vector2 pos2, Vector2 size2)
        {
            return (pos1.X + (size1.X / 2) >= pos2.X - (size2.X/2) && pos1.X - (size1.X / 2) <= pos2.X + (size2.X/2) &&
                   pos1.Y + (size1.Y / 2) >= pos2.Y - (size2.Y/2) && pos1.Y - (size1.Y / 2) <= pos2.Y + (size2.Y/2));
        }

        /// <summary>
        /// Changes the current color to the next color.
        /// </summary>
        public void ForwardColor()
        {
            //Cue c = GameRef.soundBank.GetCue("swoosh");
            //c.Play();

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

        /// <summary>
        /// Changes the current color to the previous color.
        /// </summary>
        public void BackwardColor()
        {
            //Cue c = GameRef.soundBank.GetCue("swoosh");
            //c.Play();

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

        /// <summary>
        /// Activates All-Colors mode
        /// </summary>
        public void ActivateAllColorsMode()
        {
            //PreviousColor = CurrentColor;
            //CurrentColor = Colors.AllColors;
            _allColorsMode = true;
        }

        /// <summary>
        /// Deactivates All-Colors mode
        /// </summary>
        public void DeactivateAllColorsMode()
        {
            //CurrentColor = PreviousColor;
            //PreviousColor = Colors.NoColors;
            _allColorsMode = false;
        }

        /// <summary>
        /// Checks to see if the player is too far off the level's coordinates.
        /// Restarts the level if the player is.
        /// </summary>
        private void checkPlayerDeath()
        {
            float x = player.body.Position.X;
            float y = player.body.Position.Y;

            if (x < -KILL_VAL || x > Width + KILL_VAL || y < -KILL_VAL || y > Height + KILL_VAL)
            {
                Restart();
            }
        }

        /// <summary>
        /// Adjusts the camera to display where the player is in the level
        /// </summary>
        public void AdjustCamera()
        {
            float x = (float)Math.Min(Math.Max(player.body.position.X - Globals.GameWidth / 2, 0), Width - Globals.GameWidth);
            float y = (float)Math.Min(Math.Max(player.body.position.Y - Globals.GameHeight / 2, 0), Height - Globals.GameHeight);
            CameraPosition = new Vector2(x, y);
        }
    }
}
