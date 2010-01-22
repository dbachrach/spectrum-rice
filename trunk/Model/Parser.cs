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

namespace Spectrum.Model
{
    class Parser
    {
        /**
         * Takes a file path to a JSON encoded representation of a Level and returns a Level object
         */
        public static Level Parse(string filename)
        {
            string levelJSON = FileAsString("demo.txt");

            Level level = null;

            ArrayList levelData = (ArrayList)JSON.JsonDecode(levelJSON);

            /* Parse Level */
            foreach (Hashtable obj in levelData)
            {
                string objType = (string)obj["object"];
                if (objType.Equals("level"))
                {
                    level = new Level();

                    if (!obj.ContainsKey("id") || !obj.ContainsKey("number") || !obj.ContainsKey("name") ||
                       !obj.ContainsKey("width") || !obj.ContainsKey("height") || !obj.ContainsKey("background") ||
                       !obj.ContainsKey("allowed-colors"))
                    {
                        Console.WriteLine("Level must have all properties.");
                    }

                    level.Id = (string)obj["id"];
                    level.Number = (double)obj["number"];
                    level.Name = (string)obj["name"];
                    level.Width = (double)obj["width"];
                    level.Height = (double)obj["height"];

                    ArrayList positionJson = (ArrayList)obj["start-position"];
                    level.StartPosition = new Vector2((float)((double)positionJson[0]), (float)((double)positionJson[1]));

                    //level.Background = obj["background"]; /* TODO Turn this into a Texture2D */
                    level.AllowedColors = Colors.ColorsFromJsonArray((ArrayList)obj["allowed-colors"]);
                    level.CurrentColor = Colors.GreenColor;
                }
                break;
            }

            /* Create Player */
            Player player = new Player();
            player.Container = level;
            Console.WriteLine("Start position {0},{1}", level.StartPosition.X, level.StartPosition.Y);
            player.Boundary = new Rectangle((int) level.StartPosition.X, (int) level.StartPosition.Y, 0, 0);
            level.AddPlayer(player);

            /* Create Game Edges */
            SolidGround edgeLeft = new SolidGround(1, (int) (level.Height));
            edgeLeft.SetPosition(-1, 0);
            edgeLeft.Id = "Left";
            edgeLeft.Container = level;
            level.AddGameObject(edgeLeft);

            SolidGround edgeRight = new SolidGround(1, (int)(level.Height));
            edgeRight.SetPosition((int) (level.Width + 1), 0);
            edgeRight.Id = "Right";
            edgeRight.Container = level;
            level.AddGameObject(edgeRight);

            SolidGround edgeTop = new SolidGround((int)(level.Width), 1);
            edgeTop.SetPosition(0, -1);
            edgeTop.Id = "Top";
            edgeTop.Container = level;
            level.AddGameObject(edgeTop);

            SolidGround edgeBottom = new SolidGround((int)(level.Width), 1);
            edgeBottom.SetPosition(0, (int)(level.Height + 1));
            edgeBottom.Id = "Bottom";
            edgeBottom.Container = level;
            /* TODO: Add Death on collision effect */
            level.AddGameObject(edgeBottom);

            /* Parse Game Objects and find id*/
            foreach (Hashtable obj in levelData)
            {
                GameObject newObject = null;
                string objType = (string)obj["object"];
                if (!objType.Equals("level"))
                {

                    if (objType.Equals("game-object"))
                    {
                        newObject = new GameObject();
                    }
                    else if (objType.Equals("ground"))
                    {
                        newObject = new Ground();
                    }
                    else if (objType.Equals("solid-ground"))
                    {
                        double w = (double) obj["_w"];
                        double h = (double)obj["_h"];
                        newObject = new SolidGround((int) w, (int) h);
                    }
                    else if (objType.Equals("door"))
                    {
                        newObject = new Door(player);
                    }
                    else if (objType.Equals("platform"))
                    {
                        newObject = new Platform();
                    }
                    else if (objType.Equals("block"))
                    {
                        newObject = new Block();
                    }

                    /* Set properties */
                    if (obj.ContainsKey("id"))
                    {
                        newObject.Id = (string)obj["id"];
                    }

                    newObject.Container = level;

                    level.AddGameObject(newObject);
                }
            }

            


            /* Set game object properties and resolve pointers to other objects */

            foreach (Hashtable obj in levelData)
            {
                string objID = (string)obj["id"];

                GameObject newObject = level.GameObjectForId(objID);

                if (obj.ContainsKey("colors"))
                {
                    newObject.ViewableColors = Colors.ColorsFromJsonArray((ArrayList)obj["colors"]);
                }
                if (obj.ContainsKey("image"))
                {
                    newObject.ImageName = (string)obj["image"];
                }
                if (obj.ContainsKey("position"))
                {
                    ArrayList positionJson = (ArrayList)obj["position"];
                    newObject.Boundary = new Rectangle((int)((double)positionJson[0]), (int)((double)positionJson[1]), 0, 0);
                }
                if (obj.ContainsKey("affected-by-gravity"))
                {
                    newObject.AffectedByGravity = (bool)obj["affected-by-gravity"];
                }
                if (obj.ContainsKey("velocity"))
                {
                    ArrayList velocityJson = (ArrayList)obj["velocity"];
                    newObject.Velocity = new Vector2((float)velocityJson[0], (float)velocityJson[1]);
                }
                if (obj.ContainsKey("combinable-with"))
                {
                    newObject.CombinableWith = new List<GameObject>();

                    foreach (string cID in (ArrayList)obj["combinable-with"])
                    {
                        newObject.CombinableWith.Add(level.GameObjectForId(cID));
                    }
                }
                if (obj.ContainsKey("pickupable"))
                {
                    newObject.Pickupable = (bool)obj["pickupable"];
                }
                if (obj.ContainsKey("inactive"))
                {
                    newObject.Inactive = (bool)obj["inactive"];
                }
                if (obj.ContainsKey("inactive-image"))
                {
                    // TODO: newObject.InactiveImage = obj["inactive-image"];
                }
                if (obj.ContainsKey("events"))
                {
                    newObject.Events = ParseEvents((ArrayList)(obj["events"]), level);
                }
                if (obj.ContainsKey("exists-when-not-viewed"))
                {
                    newObject.ExistsWhenNotViewed = (bool)obj["exists-when-not-viewed"];
                }

            }

            return level;
        }

        private static List<Event> ParseEvents(ArrayList events, Level level)
        {
            List<Event> evs = new List<Event>();
            foreach (Hashtable ev in events)
            {
                Event e = new Event();
                e.Type = Event.EventTypeForString((string)ev["type"]);

                e.CollisionTarget = level.GameObjectForId((string)ev["collision-target"]);
                e.DisplayName = (string)ev["display-name"];
                e.Actions = ParseActions((ArrayList)ev["actions"], level);

                evs.Add(e);
            }
            return evs;
        }

        private static List<EventAction> ParseActions(ArrayList actions, Level level)
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

                a.Receiver = level.GameObjectForId((string)ac["receiver"]);
                a.Property = (string)ac["property"];
                a.Type = EventAction.ActionTypeForString((string)ac["type"]);
                a.Value = ac["value"];

                if (ac.ContainsKey("animated"))
                    a.Animated = (bool)ac["animated"];
                if (ac.ContainsKey("animation-duration"))
                    a.AnimationDuration = (float)ac["animation-duration"];
                if (ac.ContainsKey("delay"))
                    a.Delay = (float)ac["delay"];
                if (ac.ContainsKey("repeats"))
                    a.Repeats = (bool)ac["repeats"];
                if (ac.ContainsKey("repeat-delay"))
                    a.RepeatDelay = (float)ac["repeat-delay"];

                acts.Add(a);

            }
            return acts;
        }

        private static string FileAsString(string filename)
        {
            // Both Windows and Xbox - Ensures we are looking in the game's folder.
            String path = StorageContainer.TitleLocation + "\\" + filename;
            return File.ReadAllText(path);
        }
    }
}
