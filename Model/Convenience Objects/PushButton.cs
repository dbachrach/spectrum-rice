using System;
using System.Collections.Generic;
using System.Collections;
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

namespace Spectrum.Model.Convenience_Objects
{
    class PushButtonBox : GameObject
    {
        public PushButtonBox(Player p)
            : base()
        {
            ImageNames = new List<string>() {"switchBox"};
            Scale = 0.15f;
            IsStatic = true;
        }
    }

    class PushButtonPusher : GameObject
    {
        public PushButtonPusher(Player p)
            : base()
        {
            ImageNames = new List<string>() { "switchBoxButton" };
            Scale = 0.15f;
            IsStatic = true;
            
            Event e = new Event();
            e.Type = EventType.Collision;
            e.CollisionTarget = p;
            e.Actions = new List<EventAction>();
            EventAction act = new EventAction();
            act.Receiver = this;
            act.Property = Globals.PositionProperty;
            act.Type = ActionType.Increment;
            act.Value = new ArrayList() { 0.0, 5.0 };

            e.Actions.Add(act);

            Events = new List<Event>();
            Events.Add(e);
            
        }
    }
}
