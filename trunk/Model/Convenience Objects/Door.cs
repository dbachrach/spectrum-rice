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
    class Door : GameObject
    {
		public Door(Player player) : base() {
            ImageName = "dr";
			Pickupable = false;
            IsStatic = true;

            Events = new List<Event>();

            Event e = new Event();
            e.Type = EventType.XEvent;
            e.DisplayName = "Open Door";
            e.Actions = new List<EventAction>();

            EventAction a = new EventAction();
            a.Special = "win";
            a.Receiver = player;

            e.Actions.Add(a);
            Events.Add(e);
		}
    }
}
