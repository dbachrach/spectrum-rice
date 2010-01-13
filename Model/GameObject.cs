using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public bool Pickupable { get; set; }
        public bool Inactive { get; set; }
        public Texture2D InactiveImage { get; set; }
        public List<Events> Events { get; set; }
        public bool ExistsWhenNotViewed { get; set; }
        public Level Container { get; set; }

		static int nextId = 0;
		public GameObject() {
			Id = nextId;
			nextId++;
		}
    }
}
