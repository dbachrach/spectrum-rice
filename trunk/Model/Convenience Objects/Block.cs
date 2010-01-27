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
            ImageName = "box";
			AffectedByGravity = true;
			Pickupable = true;
            Mass = 2000;
            ExistsWhenNotViewed = true; // todo: remove
		}

        public override GameObject CombineObjectWith(GameObject obj)
        {
            if (obj is Block)
            {
                Block b = new Block();
                b.Pickupable = false;
                b.OriginalPosition = body.Position;
                b.Container = Container;
                
                /* TODO: Set b's parents to be this and obj */

                return b;
            }
            return null;
        }
    }
}
