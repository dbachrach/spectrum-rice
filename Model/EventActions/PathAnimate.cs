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
    abstract class PathAnimate : EventAction
    {
        private List<Vector2> _path;
        public List<Vector2> Path {
            get { return _path; }
            set
            {
                // set the first two points on the path as the start and destination
                _path = value;
                TotalSegments = _path.Count;
            }
        }

        public Vector2 Start { get; set; }
        public Vector2 Destination { get; set; }
        public float Speed { get; set; }
        public int NextDestination { get; set; }
        public int TotalSegments { get; set; }
        public Vector2 Direction { get; set; }
        public bool HasAdjustedPath { get; set; }
        public float SegmentDistance { get; set; }

        public PathAnimate()
        {
            Path = new List<Vector2>();
            Speed = 1;
            NextDestination = 0;
            Destination = Vector2.Zero;
            HasAdjustedPath = false;
        }

        // Adjust the direction vector to point to the new destination point
        public void UpdateDirection()
        {
            Start = Receiver.body.Position;
            Direction = Destination - Start;
            SegmentDistance = Direction.Length();
            if (SegmentDistance > Globals.Epsilon)
            {
                Direction = new Vector2(Direction.X / Direction.Length(), Direction.Y / Direction.Length());
                Direction *= Speed;
            }
        }

        // Move each path point from the center of the object to its top left corner.
        // We only need to do this once, as soon as we know the Receiver's size
        public void AdjustPath()
        {
            for (int k = 0; k < Path.Count; k++)
            {
                Vector2 oldPoint = Path[k];
                Path[k] = new Vector2(oldPoint.X + Receiver.Size.X/2, oldPoint.Y + Receiver.Size.Y/2);
            }

            if (Path.Count >= 2)
            {
                Destination = Path[0];
                NextDestination = 1;
            }
            else
            {
                Console.WriteLine("Warning! PathAnimate path has less than two points!");
            }

            HasAdjustedPath = true;
        }

        public override void InnerExecute(List<EventAction> deferFuture, double curMs)
        {
            if (!HasAdjustedPath)
            {
                AdjustPath();
            }

            UpdateDirection();

            Vector2 newPosition = Receiver.body.Position;
            newPosition += Direction;
            Vector2 toDest = Destination - newPosition;
            Vector2 toStart = Start - newPosition;

            // check if we've passed the destination
            // if so, set destination to next target point
            if (SegmentDistance < Globals.Epsilon ||
                toDest.Length() < Globals.Epsilon ||
                toStart.Length() > SegmentDistance)
            {
                Receiver.body.Position = Destination;
                SelectNextWaypoint();

                UpdateDirection();
                Receiver.body.LinearVelocity = Vector2.Zero;
            }
            else
            {
                Receiver.body.IgnoreGravity = true;

                Receiver.body.Position = Receiver.body.Position + Direction;
                Receiver.PathVector = Direction;
                //Receiver.body.LinearVelocity = 100*Direction;
            }
        }

        /**
         * This function needs to be overridden by any subclasses.
         * It is called when the animation needs to pick the next target point.
         * This behavior will differ depending if we're doing a loop or a linear path 
         */
        public abstract void SelectNextWaypoint();
    }
}
