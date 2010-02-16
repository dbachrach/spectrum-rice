using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spectrum.Model.Convenience_Objects
{
    class Switch : GameObject
    {
        public Switch(Player p)
            : base()
        {
            ImageName = "switchOff";
            IsStatic = true;

            MakeSensor();

            Event e = new Event();
            e.Type = EventType.Collision;
            e.CollisionTarget = p;
            e.Actions = new List<EventAction>();
            EventAction act = new EventAction();
            act.Receiver = this;
            act.Property = Globals.ImageProperty;
            act.Type = ActionType.Change;
            act.Value = "switchOn_sm";

            e.Actions.Add(act);

            Events = new List<Event>();
            Events.Add(e);
        }
    }
}
