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
			/* TODO: Square image name */
            ImageName = "block";
			AffectedByGravity = true;
			Pickupable = true;
		}
		/* Polygon = Square, Image = DefaultBlock, Affected By Gravity = YES, Pickupable = YES */
        public Block(double id, Colors viewableColors, Vector2 position, Vector2 velocity, List<GameObject> combineObjects, List<GameObject> combinableWith, bool inactive, Texture2D inactiveImage, List<Event> events, bool existsWhenNotViewed, Level container)
           : base(id, viewableColors, null /* TODO: Square polygon */, null /* TODO: Square image name */, position, true, velocity, combineObjects, combinableWith, true, inactive, inactiveImage, events, existsWhenNotViewed, container)
        {
		}
    }
}
