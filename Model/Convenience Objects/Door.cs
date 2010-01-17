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
    class Door : GameObject
    {
		public Door() : base() {
			/* Todo: Polygon rectangle */
			/* TODO: Door image name */
            ImageName = "door";
			AffectedByGravity = false;
			Pickupable = false;
			/* TODO: X-Event complete level */
		}
    }
}
