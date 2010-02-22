using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spectrum.Model
{
    class Wall : SolidGround
    {
        public Wall() : base() {
		}
        public Wall(int w, int h)
            : base(w,h)
        {
            InitialFriction = 0.0f;
        }

        protected override void DidLoadPhysicsBody()
        {
            //geom.FrictionCoefficient = 0.0f; // TODO: Pick a good value for friction
        }
    }
}
