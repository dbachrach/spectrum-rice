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
	enum EventType { XEvent, Behavior, Collision }
	
    class Event
    {
        public EventType Type { get; set; }
		public string DisplayName { get; set; }
        public GameObject CollisionTarget { get; set; }
        public List<EventAction> Actions { get; set; }

        public Event()
        {
            DisplayName = "";
            CollisionTarget = null;
        }

        public static EventType EventTypeForString(string str) {
            if(str.Equals("x-event")) {
                return EventType.XEvent;
            }
            else if(str.Equals("behavior")) {
                return EventType.Behavior;
            }
            else if(str.Equals("collision")) {
                return EventType.Collision;
            }
            else {
                throw new Exception("Type for event was not an event type: " + str);
            }
        }
    }
}
