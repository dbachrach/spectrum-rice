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
	enum EventType { XEvent, Behavior, Collision, Separation }
	
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

        public void LoadContent(ContentManager theContentManager, GraphicsDevice theGraphicsDevice)
        {
            if (Actions != null)
            {
                foreach (EventAction a in this.Actions)
                {
                    a.LoadContent(theContentManager, theGraphicsDevice);
                }
            }
        }

        public static EventType EventTypeForString(string str) {
            if (str.Equals("x-event"))
            {
                return EventType.XEvent;
            }
            else if (str.Equals("behavior"))
            {
                return EventType.Behavior;
            }
            else if (str.Equals("collision"))
            {
                return EventType.Collision;
            }
            else if (str.Equals("separation"))
            {
                return EventType.Separation;
            }
            else
            {
                throw new Exception("Type for event was not an event type: " + str);
            }
        }

        public void Execute(List<EventAction> deferFuture)
        {
            foreach (EventAction a in Actions)
            {
                a.Execute(deferFuture);
            }
        }

        public override string ToString()
        {
            return "(event " + Type + ", '" + DisplayName + "', " + CollisionTarget + ", (actions + " + Actions.ToString() + "))";
        }
    }
}
