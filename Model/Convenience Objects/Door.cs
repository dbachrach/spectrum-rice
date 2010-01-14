﻿using System;
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
		/* Polygon = Rectangle, Image = DefaultDoor, Affected By Gravity = NO, Pickupable = NO, X-Event = Complete Level */
		public Door(int id, Colors viewableColors, Vector2 position, bool affectedByGravity, Vector2 velocity, bool inactive, Texture2D inactiveImage, List<Event> events, bool existsWhenNotViewed, Level container)
			: base(id, viewableColors, null /* TODO: Rectangle polygon */, null /* TODO: Door image */, position, affectedByGravity, velocity, null, null, false, inactive, inactiveImage, events, existsWhenNotViewed, container)
		{}
    }
}
