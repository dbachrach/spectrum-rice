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

using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Factories;

namespace Spectrum.Model
{
    class Ball : GameObject
    {
        public Ball()
            : base()
        {
            ImageNames = new List<string>() { "ball" };
            Mass = 100;
            //Tangibility = Colors.AllColors;
            Scale = 0.15f;

            IsFixed = false;

            InitialFriction = .3f;
        }

        public override Body CreateBody(Vector2 size)
        {
            //return BodyFactory.Instance.CreateRectangleBody(Container.Sim, size.X, size.Y, Mass);
            return BodyFactory.Instance.CreateCircleBody(Container.Sim, size.X / 2, Mass);
        }
        public override Geom CreateGeom(Vector2 size)
        {
            //return GeomFactory.Instance.CreateRectangleGeom(Container.Sim, body, size.X, size.Y);
            return GeomFactory.Instance.CreateCircleGeom(Container.Sim, body, size.X / 2, 25);
        }

        public override void Update(GameTime theGameTime)
        {
            base.Update(theGameTime);

            Console.WriteLine("ball at " + this.body.position);
        }
    }

    class DeadlyBall : Ball
    {
        public DeadlyBall(Player player)
            : base()
        {
            ImageNames = new List<string>() { "deadlyball" };

            MakeDeadly(player);
        }
    }
}
