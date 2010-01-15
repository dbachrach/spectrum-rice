using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Spectrum
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Level level;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

			string levelJSON = "true";
			ArrayList<HashTable> levelData = JSON.JsonDecode(levelJSON);
			
			/* Parse Level */
			foreach (HashTable obj in levelData) {
				string objType = obj["object"];
				if (objType.Equals("level")) {
					level = new level();
					
					if(!obj.containsKey("id") || !obj.containsKey("number") || !obj.containsKey("name") ||
					   !obj.containsKey("width") || !obj.containsKey("height") || !obj.containsKey("background") ||
					   !obj.containsKey("allowed-colors")) {
						Console.WriteLine("Level must have all properties.");
					}
					
					level.Id = obj["id"];
	 				level.Number = obj["number"];
	 				level.Name = obj["name"];
	 				level.Width = obj["width"];
	 				level.Height = obj["height"];
	 				level.Background = obj["background"]; /* Turn this into a Textur2D */
	 				level.AllowedColors = obj["allowed-colors"]; /* TODO: create Colors object from array */
				}
				break;
			}	
		
			/* Parse Game Objects */
			foreach (HashTable obj in levelData) {
				object newObject;
				string objType = obj["object"];
				if (!objType.Equals("level")) {
			
					if (objType.Equals("game-object")) {
						newObject = new GameObject();
					}
					else if (objType.Equals("ground")) {
						newObject = new Ground();
					}
					else if (objType.Equals("solid-ground")) {
						newObject = new SolidGround();
					}
					else if (objType.Equals("door")) {
						newObject = new Door();
					}
					else if (objType.Equals("platform")) {
						newObject = new Platform();
					}
					
					/* Set properties */
					if(obj.containsKey("id"))
						Id = obj["id"];
					if(obj.containsKey("colors"))
						ViewableColors = obj["colors"];
					if(obj.containsKey("polygon"))
						Polygon = obj["polygon"];
					if(obj.containsKey("image"))
						Image = obj["image"];
					if(obj.containsKey("position"))
						Position = obj["position"];
					if(obj.containsKey("affected-by-gravity"))
						AffectedByGravity = obj["affected-by-gravity"];
					if(obj.containsKey("velocity"))
						Velocity = obj["velocity"];
					if(obj.containsKey("combinable-with"))
						CombinableWith = obj["combinable-with"];
					if(obj.containsKey("pickupable"))
						Pickupable = obj["pickupable"];
					if(obj.containsKey("inactive"))
						Inactive = obj["inactive"];
					if(obj.containsKey("inactive-image"))
						InactiveImage = obj["inactive-image"];
					if(obj.containsKey("events"))
						Events = obj["events"];
					if(obj.containsKey("exists-when-not-viewed"))
						ExistsWhenNotViewed = obj["exists-when-not-viewed"];
					
					Container = level;
				}
			}

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            level.LoadContent(this.Content, "sunset");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            level.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
