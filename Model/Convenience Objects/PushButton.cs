using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spectrum.Model.Convenience_Objects
{
    class PushButton : GameObject
    {
        public PushButton(Player p)
            : base()
        {
            ImageName = "switchBox";
            IsStatic = true;
            Scale = 0.1f;

            Event e = new Event();
            e.Type = EventType.Collision;
            e.CollisionTarget = p;
            e.Actions = new List<EventAction>();
            EventAction act = new EventAction();
            act.Receiver = this;
            act.Property = Globals.ImageProperty;
            act.Type = ActionType.Change;
            act.Value = "switchBoxButton"; // TODO: Make this a corrrect event action

            e.Actions.Add(act);

            Events = new List<Event>();
            Events.Add(e);
        }
    }
}
