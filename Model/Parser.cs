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
using Spectrum.Model.Convenience_Objects;
using Spectrum.Model.EventActions;

namespace Spectrum.Model
{
    class Parser
    {
        /**
         * Takes a file path to a JSON encoded representation of a Level and returns a Level object
         */
        public static Level Parse(string filename)
        {
            string levelJSON = FileAsString(filename);

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
                       !obj.ContainsKey("allowed-colors") || !obj.ContainsKey("starting-color"))
                    {
                        Console.WriteLine("Level must have all properties.");
                    }

                    level.Id = (string)obj["id"];
                    level.Number = (double)obj["number"];
                    level.Name = (string)obj["name"];
                    Console.WriteLine("Level name {0}", level.Name);
                    level.Width = (double)obj["width"];
                    level.Height = (double)obj["height"];

                    ArrayList positionJson = (ArrayList)obj["start-position"];
                    level.StartPosition = new Vector2((float)((double)positionJson[0]), (float)((double)positionJson[1]));

                    
                    ArrayList cameraJson = (ArrayList)obj["camera"];
                    if (cameraJson != null)
                    {
                        level.CameraPosition = new Vector2((float)((double)cameraJson[0]), (float)((double)cameraJson[1]));
                    }
                    level.StartingColor = Colors.ColorsFromJsonArray((ArrayList)obj["starting-color"]);
                    level.BackgroundImageName = (string) obj["background"];
                    level.AllowedColors = Colors.ColorsFromJsonArray((ArrayList)obj["allowed-colors"]);
                    level.CurrentColor = level.StartingColor;
                }
                break;
            }

            /* Create Player */
            Player player = new Player();
            player.Container = level;
            Console.WriteLine("Start position {0},{1}", level.StartPosition.X, level.StartPosition.Y);
            player.OriginalPosition = level.StartPosition;
            level.AddPlayer(player);

            int borderWidth = 150;

            /* Create Game Edges */
            Wall edgeLeft = new Wall(borderWidth, (int)(level.Height));
            edgeLeft.OriginalPosition = new Vector2(-borderWidth, 0);
            edgeLeft.Id = "Left";
            edgeLeft.Container = level;
            level.AddGameObject(edgeLeft);

            Wall edgeRight = new Wall(borderWidth, (int)(level.Height));
            edgeRight.OriginalPosition = new Vector2((int)(level.Width + 1), 0);
            edgeRight.Id = "Right";
            edgeRight.Container = level;
            level.AddGameObject(edgeRight);

            Wall edgeTop = new Wall((int)(level.Width), borderWidth);
            edgeTop.OriginalPosition = new Vector2(0, -borderWidth);
            edgeTop.Id = "Top";
            edgeTop.Container = level;
            level.AddGameObject(edgeTop);

            
            Wall edgeBottom = new Wall((int)(level.Width), borderWidth);
            // TODO: edgeBottom.OneTime = true;
            edgeBottom.OriginalPosition = new Vector2(0, (int)(level.Height + 1));
            edgeBottom.Id = "Bottom";
            edgeBottom.Container = level;
            edgeBottom.Tangibility = Colors.NoColors;
            edgeBottom.PlayerTangibility = Colors.NoColors;
            edgeBottom.Sensibility = Colors.AllColors;
            edgeBottom.PlayerSensibility = Colors.AllColors;

            // Add Death on collision effect 
            edgeBottom.MakeDeadly();

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
                        double w = (double)obj["_w"];
                        double h = (double)obj["_h"];
                        newObject = new Ground((int)w, (int)h);
                    }
                    else if (objType.Equals("door"))
                    {
                        newObject = new Door(player);
                    }
                    else if (objType.Equals("platform"))
                    {
                        int w = 0;
                        if (obj.ContainsKey("_w"))
                        {
                            w = (int)(double)obj["_w"];
                        }
                        
                        newObject = new Platform(w);
                    }
                    else if (objType.Equals("block"))
                    {
                        newObject = new Block();
                    }
                      /*  
                    else if (objType.Equals("sensor"))
                    {
                        double w = (double)obj["_w"];
                        double h = (double)obj["_h"];
                        newObject = new Sensor((int) w, (int) h);
                    }*/
                         
                    else if (objType.Equals("switch"))
                    {
                        newObject = new Switch(player);
                    }
                    else if (objType.Equals("push"))
                    {
                        newObject = new PushButton(player);
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

                if (objID.Equals("level"))
                {
                    continue;
                }

                GameObject newObject = level.GameObjectForId(objID);

                if (obj.ContainsKey("colors"))
                {
                    newObject.setVisibility(Colors.ColorsFromJsonArray((ArrayList)obj["colors"]));
                }
                if (obj.ContainsKey("tangibility"))
                {
                    newObject.Tangibility = Colors.ColorsFromJsonArray((ArrayList)obj["tangibility"]);
                }
                if (obj.ContainsKey("player-tangibility"))
                {
                    newObject.PlayerTangibility = Colors.ColorsFromJsonArray((ArrayList)obj["player-tangibility"]);
                }
                if (obj.ContainsKey("sensibility"))
                {
                    newObject.Sensibility = Colors.ColorsFromJsonArray((ArrayList)obj["sensibility"]);
                }
                if (obj.ContainsKey("player-sensibility"))
                {
                    newObject.PlayerSensibility = Colors.ColorsFromJsonArray((ArrayList)obj["player-sensibility"]);
                }
                if (obj.ContainsKey("image"))
                {
                    newObject.ImageName = (string)obj["image"];
                }
                if (obj.ContainsKey("scale"))
                {
                    newObject.Scale = (float)(double)obj["scale"];
                }
                if (obj.ContainsKey("position"))
                {
                    ArrayList positionJson = (ArrayList)obj["position"];
                    newObject.OriginalPosition = new Vector2((int)((double)positionJson[0]), (int)((double)positionJson[1]));
                }
                if (obj.ContainsKey("static"))
                {
                    newObject.IsStatic = (bool)obj["static"];
                }
                if (obj.ContainsKey("mass"))
                {
                    newObject.Mass = (float)(double)obj["mass"];
                }
                if (obj.ContainsKey("velocity"))
                {
                    ArrayList velocityJson = (ArrayList)obj["velocity"];
                    newObject.OriginalVelocity = new Vector2((float)velocityJson[0], (float)velocityJson[1]);
                }
                if (obj.ContainsKey("combinable-with"))
                {
                    //newObject.CombinableWith = new List<GameObject>();

                    foreach (string cID in (ArrayList)obj["combinable-with"])
                    {
                        newObject.CombinableWith.Add(level.GameObjectForId(cID));
                    }
                }
                if (obj.ContainsKey("pickupable"))
                {
                    newObject.Pickupable = (bool)obj["pickupable"];
                }
                if (obj.ContainsKey("events"))
                {
                    List<Event> parsedEvents = ParseEvents((ArrayList)(obj["events"]), level);
                    if (newObject.Events == null || newObject.Events.Count == 0)
                    {
                        newObject.Events = parsedEvents;
                    }
                    else
                    {
                        newObject.Events.AddRange(parsedEvents);
                    }
                }
                if (obj.ContainsKey("zindex"))
                {
                    newObject.ZIndex = (int)(double)obj["zindex"];
                }
                if (obj.ContainsKey("deadly"))
                {
                    bool isDeadly = (bool)obj["deadly"];
                    if (isDeadly)
                    {
                        newObject.MakeDeadly();
                    }
                }
                if (obj.ContainsKey("_switch-actions"))
                {
                    
                    if (newObject is Switch)
                    {
                        List<EventAction> parsedActions = ParseActions((ArrayList)obj["_switch-actions"], level);
                        newObject.Events[0].Actions.AddRange(parsedActions);
                    }
                }
                if (obj.ContainsKey("friction"))
                {
                    newObject.InitialFriction = (float)(double)obj["friction"];
                }
                if (obj.ContainsKey("bounce"))
                {
                    newObject.InitialBounciness = (float)(double)obj["bounce"];
                }
                if (obj.ContainsKey("drag"))
                {
                    newObject.InitialLinearDrag = (float)(double)obj["drag"];
                }
            }

            level.GameObjects.Sort();

            return level;
        }

        private static List<Event> ParseEvents(ArrayList events, Level level)
        {
            List<Event> evs = new List<Event>();
            foreach (Hashtable ev in events)
            {
                Event e = new Event();
                e.Type = Event.EventTypeForString((string)ev["type"]);

                string colTar = (string)ev["collision-target"];
                if (colTar != null)
                {
                    e.CollisionTarget = level.GameObjectForId(colTar);
                }
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
                EventAction a;
                if (!ac.ContainsKey("receiver") || !ac.ContainsKey("property") || !ac.ContainsKey("type")
                    || !ac.ContainsKey("value"))
                {
                    Console.WriteLine("Action must have all required properties.");
                }

                if (((string)ac["type"]) == Globals.AnimationAction)
                {
                    PathAnimate animation;

                    if (ac.ContainsKey("animation-type") && ((string)ac["animation-type"]) == "linear")
                    {
                        animation = new LinearPathAnimate();
                    }
                    else
                    {
                        animation = new LoopPathAnimate();
                    }

                    List<Vector2> path = new List<Vector2>();

                    ArrayList points = (ArrayList)ac["value"];
                    foreach (var pt in points)
                    {
                        ArrayList point = (ArrayList)pt;

                        Vector2 newPoint = new Vector2((float)(double)point[0], (float)(double)point[1]);
                        path.Add(newPoint);
                    }

                    animation.Path = path;

                    if (ac.ContainsKey("speed"))
                    {
                        animation.Speed = (float)(double)ac["speed"];
                    }

                    a = animation;
                }
                else
                {
                    a = new EventAction();
                }

                a.Receiver = level.GameObjectForId((string)ac["receiver"]);
                a.Property = (string)ac["property"];
                a.Type = EventAction.ActionTypeForString((string)ac["type"]);
                a.Value = ac["value"];

                if (ac.ContainsKey("animated"))
                    a.Animated = (bool)ac["animated"];
                if (ac.ContainsKey("animation-duration"))
                    a.AnimationDuration = (float)((double)ac["animation-duration"]);
                if (ac.ContainsKey("delay"))
                    a.Delay = (float)((double)ac["delay"]);
                if (ac.ContainsKey("repeats"))
                    a.Repeats = (bool)ac["repeats"];
                if (ac.ContainsKey("repeat-delay"))
                    a.RepeatDelay = (float)((double)ac["repeat-delay"]);
                if (ac.ContainsKey("repeat-count"))
                    a.RepeatCount = (int)((double)ac["repeat-count"]);
                if (ac.ContainsKey("special"))
                    a.Special = (string)ac["special"];

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
