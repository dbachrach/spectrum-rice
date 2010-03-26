using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spectrum.Model.Convenience_Objects
{
    class Funnel : GameObject
    {
        public Funnel()
            : base()
        {
            ImageNames = new List<string> { "funnel-bg", "funnel","funnel-indicator" };
            IsStatic = true;
            Scale = .5f;
        }
    }
}
