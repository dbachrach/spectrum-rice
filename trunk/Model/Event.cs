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
	enum EventType { EventTypeXEvent, EventTypeBehvaior, EventTypeCollision }
	
    class Event
    {
        public EventType Type { get; set; }
		public string DisplayName { get; set; }
        public GameObject CollisionTarget { get; set; }
		public List<Action> Actions { get; set; }

        public Event()
        {
            DisplayName = "";
        }
    }
}
