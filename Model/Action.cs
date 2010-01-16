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
	enum ActionType { ActionTypeChange, ActionTypeIncrement, ActionTypeDecrement }
	
    class Action
    {
        public GameObject Receiver { get; set; }
		public string Property { get; set; }
		public ActionType Type { get; set; }
		public string Value { get; set; }
		public bool Animated { get; set; }
		public float AnimationDuration { get; set; }
		public float Delay { get; set; }
		public bool Repeats { get; set; }
		public float RepeatDelay { get; set; }

		/* Default constructor. */
		public Action() {
			Animated = false;
			AnimationDuration = 1;
			Delay = 0;
			Repeats = false;
			RepeatDelay = 0;
		}
    }
}
