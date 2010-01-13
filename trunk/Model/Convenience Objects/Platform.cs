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
		/* Convenience to create an object with Pickupable = NO */
		public Platform(int id, Colors viewableColors, Polygon polygon, Texture2D image, Vector2 position, bool affectedByGravity, Vector2 velocity, List<GameObject> combineObjects, List<GameObject> combinableWith, bool inactive, Texture2D inactiveImage, List<Event> events, bool existsWhenNotViewed, Level container)
			: base(id, viewableColors, polygon, image, position, affectedByGravity, velocity, combineObjects, combinableWith, false, inactive, inactiveImage, events, existsWhenNotViewed, container)
		{}
    }
}
