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
    class GameObject
    {
        public int Id { get; set; }
        public Colors ViewableColors { get; set; }
        public Polygon Polygon { get; set; }
        public Texture2D Image { get; set; }
        public Vector2 Position { get; set; }
        public bool AffectedByGravity { get; set; }		
        public Vector2 Velocity { get; set; }
        public List<GameObject> CombineObjects { get; set; }
		public List<GameObject> CombinableWith { get; set; }
        public bool Pickupable { get; set; }
        public bool Inactive { get; set; }
        public Texture2D InactiveImage { get; set; }
        public List<Event> Events { get; set; }
        public bool ExistsWhenNotViewed { get; set; }
        public Level Container { get; set; }

		public GameObject(int id, Colors viewableColors, Polygon polygon, Texture2D image, 
            Vector2 position, bool affectedByGravity, Vector2 velocity, List<GameObject> combineObjects, 
            List<GameObject> combinableWith, bool pickupable, bool inactive, Texture2D inactiveImage, 
            List<Event> events, bool existsWhenNotViewed, Level container) {
			Id = id;
			ViewableColors = viewableColors;
			Polygon = polygon;
			Image = image;
			Position = position;
			AffectedByGravity = affectedByGravity;
			Velocity = velocity;
			CombineObjects = combineObjects;
			CombinableWith = combinableWith;
			Pickupable = pickupable;
			Inactive = inactive;
			InactiveImage = inactiveImage;
			Events = events;
			ExistsWhenNotViewed = existsWhenNotViewed;
			Container = container;
		}

        public bool currentlyVisible()
        {
            return Container.CurrentColor.contains(this.ViewableColors);
        }
    }
}
