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
    class Block : GameObject
    {
		public Block() : base() {
			/* TODO: Square polygon */
            ImageName = "box";
			AffectedByGravity = true;
			Pickupable = true;
            //Boundary = new Rectangle(0, 0, 50, 53);
		}
    }
}
