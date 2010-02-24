using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spectrum.Model
{
    class Wall : Ground
    {
        public Wall() : base() {
		}
        public Wall(int w, int h)
            : base(w,h)
        {
            InitialFriction = 0.0f;
        }
    }
}
