using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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

		    //string levelJSON = "[{\"object\" : \"level\",\"id\" : 1,\"number\" : 1,\"name\" : \"Demo Level\",\"width\" : 500,\"height\" : 500,\"allowed-colors\" : [\"all\"], \"start-position\" : [50,300] }{\"object\" : \"block\",\"id\" : 2,\"position\" : [275, 350],\"colors\" : [\"yellow\"]}{\"object\" : \"block\",\"id\" : 3,\"position\" : [325, 350],\"colors\" : [\"blue\"]} {\"object\" : \"platform\",\"id\" : 4,\"colors\" : [\"green\"],\"position\" : [400, 200],\"affected-by-gravity\" : false,\"exists-when-not-viewed\" : false}{\"object\" : \"game-object\",\"id\" : 5,\"colors\" : [\"green\"],\"position\" : [425, 187],\"image\" : \"switch\",\"affected-by-gravity\" : false,\"exists-when-not-viewed\" : false}]";
            string levelJSON = FileAsString("demo.txt");


            ArrayList levelData = (ArrayList) JSON.JsonDecode(levelJSON);
			
			/* Parse Level */
			foreach (Hashtable obj in levelData) {
				string objType = (string) obj["object"];
				if (objType.Equals("level")) {
					level = new Level();

                    if (!obj.ContainsKey("id") || !obj.ContainsKey("number") || !obj.ContainsKey("name") ||
                       !obj.ContainsKey("width") || !obj.ContainsKey("height") || !obj.ContainsKey("background") ||
                       !obj.ContainsKey("allowed-colors"))
                    {
						Console.WriteLine("Level must have all properties.");
					}
					
					level.Id = (double) obj["id"];
	 				level.Number = (double) obj["number"];
	 				level.Name = (string) obj["name"];
	 				level.Width = (double) obj["width"];
	 				level.Height = (double) obj["height"];

                    ArrayList positionJson = (ArrayList)obj["start-position"];
                    level.StartPosition = new Vector2((float)((double)positionJson[0]), (float)((double)positionJson[1]));

	 				//level.Background = obj["background"]; /* TODO Turn this into a Textur2D */
                    level.AllowedColors = Colors.ColorsFromJsonArray((ArrayList) obj["allowed-colors"]);
                    level.CurrentColor = Colors.GreenColor;
				}
				break;
			}

            Console.WriteLine("Level: " + level);
		
			/* Parse Game Objects */
			foreach (Hashtable obj in levelData) {
				GameObject newObject = null;
				string objType = (string) obj["object"];
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
                    else if (objType.Equals("block"))
                    {
                        newObject = new Block();
                    }
					
					/* Set properties */
                    if (obj.ContainsKey("id"))
                        newObject.Id = (double) obj["id"];
                    if (obj.ContainsKey("colors"))
                        newObject.ViewableColors = Colors.ColorsFromJsonArray((ArrayList)obj["colors"]);
                    //if (obj.ContainsKey("polygon"))
                        // TODO: newObject.Polygon = obj["polygon"];
                    if (obj.ContainsKey("image"))
                        newObject.ImageName = (string)obj["image"];
                    if (obj.ContainsKey("position"))
                    {
                        ArrayList positionJson = (ArrayList) obj["position"];
                        newObject.Position = new Vector2((float)((double)positionJson[0]), (float)((double)positionJson[1]));
                    }
                    if (obj.ContainsKey("affected-by-gravity"))
                        newObject.AffectedByGravity = (bool) obj["affected-by-gravity"];
                    if (obj.ContainsKey("velocity"))
                    {
                        ArrayList velocityJson = (ArrayList)obj["velocity"];
                        newObject.Velocity = new Vector2((float)velocityJson[0], (float)velocityJson[1]);
                    }
                    //if (obj.ContainsKey("combinable-with"))
                        // TODO: newObject.CombinableWith = obj["combinable-with"];
                    if (obj.ContainsKey("pickupable"))
                        newObject.Pickupable = (bool) obj["pickupable"];
                    if (obj.ContainsKey("inactive"))
                        newObject.Inactive = (bool) obj["inactive"];
                    //if (obj.ContainsKey("inactive-image"))
                        // TODO: newObject.InactiveImage = obj["inactive-image"];
                    if (obj.ContainsKey("events"))
                        newObject.Events = ParseEvents( (ArrayList) (obj["events"]));
                    if (obj.ContainsKey("exists-when-not-viewed"))
                        newObject.ExistsWhenNotViewed = (bool) obj["exists-when-not-viewed"];

                    newObject.Container = level;

                    level.AddGameObject(newObject);
				}
			}

            /* Create Player */
            Player player = new Player();
            player.Container = level;
            player.Position = level.StartPosition;
            level.AddPlayer(player);
            
            

            /* TODO: Go through all objects and peice together their pointers to other objects */

            base.Initialize();
        }
        
        private string FileAsString(string filename)
        {
            // Both Windows and Xbox - Ensures we are looking in the game's folder.
            String path = StorageContainer.TitleLocation + "\\" + filename;
            return File.ReadAllText(path);
            /*
            try
            {
                String text = "";
                StreamReader streamReader = new StreamReader(path);
                String line;

                while ((line = streamReader.ReadLine()) != null)
                {
                    text += line;
                }

                streamReader.Close();
                return line;
            }
            catch (Exception e)
            {
                // Do things here incase it can't read the file
                Console.WriteLine("File Exception: " + e);
                return "";
            }
             */
        }
        
        private List<Event> ParseEvents(ArrayList events)
        {
            List<Event> evs = new List<Event>();
            foreach (Hashtable ev in events)
            {
                Event e = new Event();
                e.Type = Event.EventTypeForString((string)ev["type"]);
                // TODO: Parse this to an object e.CollisionTarget = ev["collision-target"];
                e.DisplayName = (string) ev["display-name"];
                e.Actions = ParseActions((ArrayList)ev["actions"]);
            }
            return evs;
        }

        private List<EventAction> ParseActions(ArrayList actions)
        {
            List<EventAction> acts = new List<EventAction>();
            foreach (Hashtable ac in actions)
            {
                EventAction a = new EventAction();
                if (!ac.ContainsKey("receiver") || !ac.ContainsKey("property") || !ac.ContainsKey("type")
                    || !ac.ContainsKey("value"))
                {
                    Console.WriteLine("Action must have all required properties.");
                }
                // TODO: Parse this to an object a.Receiver = ac["receiver"]; 
                a.Property = (string)ac["property"];
                a.Type = EventAction.ActionTypeForString((string)ac["type"]);
                a.Value = (string)ac["value"];

                if(ac.ContainsKey("animated"))
                    a.Animated = (bool) ac["animated"];
                if(ac.ContainsKey("animation-duration"))
                    a.AnimationDuration = (float) ac["animation-duration"];
                if(ac.ContainsKey("delay"))
                    a.Delay = (float) ac["delay"];
                if(ac.ContainsKey("repeats"))
                    a.Repeats = (bool) ac["repeats"];
                if(ac.ContainsKey("repeat-delay"))
                    a.RepeatDelay = (float) ac["repeat-delay"];
                
            }
            return acts;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            level.LoadContent(this.Content, GraphicsDevice);
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
         
            level.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(level.CurrentColor.SystemColor());

            spriteBatch.Begin();
            level.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
