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
			Pickupable = true;
            Mass = 2000;
            Tangibility = Colors.AllColors;
            Scale = 0.15f;
		}

        public override GameObject CombineObjectWith(GameObject obj)
        {
            if (obj is Block)
            {
                Block b = new Block();
                b.Pickupable = false;

                Vector2 v = body.Position;
                v.X = v.X - (obj.Size.X / 2);
                v.Y = v.Y - (obj.Size.Y / 2);

                b.OriginalPosition = v;
                
                b.Container = Container;
                Colors newColor = this.Visibility.ColorByMixingWith(obj.Visibility);

                b.setVisibility(newColor);
                b.Tangibility = Colors.AllColors;

                return b;
            }
            return null;
        }

        protected override void DidLoadPhysicsBody()
        {
            geom.FrictionCoefficient = .8f; // TODO: Pick a good value for friction
        }
    }
}
