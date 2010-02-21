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
            Scale = 0.1f;

            Tangibility = Colors.NoColors;
            PlayerTangibility = Colors.NoColors;
            Sensibility = Colors.AllColors;
            PlayerSensibility = Colors.AllColors;

            Event e = new Event();
            e.Type = EventType.Collision;
            e.CollisionTarget = p;
            e.Actions = new List<EventAction>();
            EventAction act = new EventAction();
            act.Receiver = this;
            act.Property = Globals.ImageProperty;
            act.Type = ActionType.Change;
            act.Value = "switchOn";

            e.Actions.Add(act);

            Events = new List<Event>();
            Events.Add(e);
        }

        public override void setVisibility(Colors vis)
        {
            Visibility = vis;
            PlayerSensibility = vis;
        }
    }
}
