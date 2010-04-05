using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spectrum.Model
{
    class DeadlyCloud : GameObject
    {
        public DeadlyCloud(Player player)
            : base()
        {
            ImageNames = new List<string>() { "deadlyclouds" };
            Pickupable = false;
            IsStatic = true;
            ZIndex = 100; // draw below player
            Scale = 0.5f;

            Events = new List<Event>();
            MakeDeadly(player);
        }
    }
}
