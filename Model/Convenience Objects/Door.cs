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
            ImageNames = new List<string>() {"door"};
			Pickupable = false;
            IsStatic = true;
            ZIndex = 100; // draw below player
            Scale = .2f;

            Tangibility = Colors.NoColors;
            PlayerTangibility = Colors.NoColors;
            Sensibility = Colors.AllColors;
            PlayerSensibility = Colors.AllColors;

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

        public override void setVisibility(Colors vis)
        {
            Visibility = vis;
            PlayerSensibility = vis;
        }
    }
}
