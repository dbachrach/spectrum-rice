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

namespace Spectrum.Model
{
    class Ground : GameObject
    {
		public Ground() : base() {
			AffectedByGravity = false;
			Pickupable = false;
            ImageName = null;
            IsStatic = true;
		}

        protected override void DidLoadPhysicsBody()
        {
            geom.FrictionCoefficient = .6f; // TODO: Pick a good value for friction
        }
    }
}
