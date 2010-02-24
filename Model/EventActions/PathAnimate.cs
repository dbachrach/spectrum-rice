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
        private List<Vector2> _path;
        public List<Vector2> Path {
            get { return _path; }
            set
            {
                // set the first two points on the path as the start and destination
                _path = value;
                TotalSegments = _path.Count;
                if (_path.Count >= 2)
                {
                    Start = _path[0];
                    Destination = _path[1];
                    NextDestination = 2 % TotalSegments;
                    UpdateDirection();
                }
                else
                {
                    Console.WriteLine("Warning! PathAnimate path has less than two points!");
                }
            }
        }
        
        public Vector2 Start { get; set; }
        public Vector2 Destination { get; set; }
        public float Speed { get; set; }
        public int NextDestination { get; set; }
        public int TotalSegments { get; set; }
        public Vector2 Direction { get; set; }

        public PathAnimate()
        {
            Path = new List<Vector2>();
            Speed = 1;
            NextDestination = 0;
            Start = Vector2.Zero;
            Destination = Vector2.Zero;
        }

        public void UpdateDirection()
        {
            Direction = Destination - Start;
            Direction.Normalize();
            Direction *= Speed;
        }

        public override void InnerExecute(List<EventAction> deferFuture, double curMs)
        {
            Vector2 newPosition = Receiver.body.Position;
            Vector2 toDest = Destination - newPosition;

            // check if we've passed the destination
            if (Math.Sign(Direction.X) != Math.Sign(toDest.X))
            {
                Start = Destination;
                Destination = Path[NextDestination];
                NextDestination = (NextDestination + 1) % TotalSegments;
                UpdateDirection();
            }

        }
    }
}
