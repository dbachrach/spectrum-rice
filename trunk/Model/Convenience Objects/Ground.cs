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
		/* Convenience to create an object with Affected By Gravity = NO, Pickupable = NO */
		public Ground(int id, Colors viewableColors, Polygon polygon, Texture2D image, Vector2 position, Vector2 velocity, List<GameObject> combineObjects, List<GameObject> combinableWith, bool inactive, Texture2D inactiveImage, List<Events> events, bool existsWhenNotViewed, Level container) {
			
			base(id, viewableColors, polygon, image, position, false, velocity, combineObjects, combinableWith, false, inactive, inactiveImage, events, existsWhenNotViewed, container)
		}
    }
}
