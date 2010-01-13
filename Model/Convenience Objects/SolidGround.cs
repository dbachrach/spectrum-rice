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
    class SolidGround : GameObject
    {
		/* Convenience to create an object with Affected By Gravity = NO, Pickupable = NO, Viewable in All Colors, and Exists When Not Viewed */
		public SolidGround(int id, Polygon polygon, Texture2D image, Vector2 position, Vector2 velocity, List<GameObject> combineObjects, List<GameObject> combinableWith, List<Event> events, Level container)
            : base(id, Colors.AllColors, polygon, image, position, false, velocity, combineObjects, combinableWith, false, false, null, events, true, container)
        {
		}
    }
}
