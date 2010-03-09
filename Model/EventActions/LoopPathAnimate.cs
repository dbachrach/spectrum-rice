using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spectrum.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace Spectrum.Model.EventActions
{
    class LoopPathAnimate : PathAnimate
    {
        public LoopPathAnimate() : base()
        {
        }

        public override void SelectNextWaypoint()
        {
            Destination = Path[NextDestination];
            NextDestination = (NextDestination + 1) % TotalSegments;
        }
    }
}
