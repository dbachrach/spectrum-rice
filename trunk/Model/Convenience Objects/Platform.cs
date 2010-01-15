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
    class Platform : GameObject
    {
		public Platform() : base() {
			// TODO: Add default image for a platform
		}
		// Convenience to create an object with Pickupable = NO
        public Platform(double id, Colors viewableColors, Polygon polygon, string imageName, Vector2 position, bool affectedByGravity, Vector2 velocity, List<GameObject> combineObjects, List<GameObject> combinableWith, bool inactive, Texture2D inactiveImage, List<Event> events, bool existsWhenNotViewed, Level container)
			: base(id, viewableColors, polygon, imageName, position, affectedByGravity, velocity, combineObjects, combinableWith, false, inactive, inactiveImage, events, existsWhenNotViewed, container)
		{}
    }
}
