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
    class PathAnimate : EventAction
    {
        public List<Vector2> Path { get; set; }
        public float Speed { get; set; }

        public PathAnimate()
        {
            Path = new List<Vector2>();
            Speed = 1;
        }

        public override void Execute(List<EventAction> deferFuture, double curMs)
        {

        }
    }
}
