﻿using System;
using System.Collections;
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
	enum ActionType { Change, Increment, Decrement, AddColors, RemoveColors }
	
    class EventAction
    {
        public GameObject Receiver { get; set; }
		public string Property { get; set; }
		public ActionType Type { get; set; }
		public object Value { get; set; }
		public bool Animated { get; set; }
		public float AnimationDuration { get; set; }
		public float Delay { get; set; }
		public bool Repeats { get; set; }
		public float RepeatDelay { get; set; }
        public string Special { get; set; } 

		/* Default constructor. */
        public EventAction()
        {
			Animated = false;
			AnimationDuration = 1;
			Delay = 0;
			Repeats = false;
			RepeatDelay = 0;
            Special = "";
		}

        public static ActionType ActionTypeForString(string str)
        {
            if (str.Equals(Globals.ChangeAction))
            {
                return ActionType.Change;
            }
            else if (str.Equals(Globals.IncrementAction))
            {
                return ActionType.Increment;
            }
            else if (str.Equals(Globals.DecrementAction))
            {
                return ActionType.Decrement;
            }
            else if (str.Equals(Globals.AddColorsAction))
            {
                return ActionType.AddColors;
            }
            else if (str.Equals(Globals.RemoveColorsAction))
            {
                return ActionType.RemoveColors;
            }
            else
            {
                throw new Exception("Type for action was not an action type: " + str);
            }
        }

        public void Execute()
        {
            /* TODO: All execution properties */
            if (Special != null && !Special.Equals(""))
            {
                if (Special.Equals(Globals.WinSpecial))
                {
                    Console.WriteLine("WIN");
                    Player p = (Player) this.Receiver;
                    p.WinLevel();
                }
                else if (Special.Equals(Globals.LoseSpecial))
                {
                    Console.WriteLine("LOSE");
                    Player p = (Player)this.Receiver;
                    p.LoseLevel();
                }
            }
            else if (Property.Equals(Globals.ColorsProperty))
            {
                /* TODO: Indicate this to user with flash of light or something */
                switch (Type)
                {
                    case ActionType.Change:
                        Receiver.setVisibility(Colors.ColorsFromJsonArray((ArrayList)Value));
                        break;
                    case ActionType.AddColors:
                        Receiver.setVisibility(Receiver.Visibility.Combine(Colors.ColorsFromJsonArray((ArrayList)Value)));
                        break;
                    case ActionType.RemoveColors:
                        Receiver.setVisibility(Receiver.Visibility.Difference(Colors.ColorsFromJsonArray((ArrayList)Value)));
                        break;
                }
            }
            else if (Property.Equals(Globals.PlayerTangibilityProperty))
            {
                /* TODO: Indicate this to user with flash of light or something */
                switch (Type)
                {
                    case ActionType.Change:
                        Receiver.PlayerTangibility = Colors.ColorsFromJsonArray((ArrayList)Value);
                        break;
                    case ActionType.AddColors:
                        Receiver.PlayerTangibility = Receiver.Visibility.Combine(Colors.ColorsFromJsonArray((ArrayList)Value));
                        break;
                    case ActionType.RemoveColors:
                        Receiver.PlayerTangibility = Receiver.Visibility.Difference(Colors.ColorsFromJsonArray((ArrayList)Value));
                        break;
                }
            }
            else if (Property.Equals(Globals.ImageProperty)) {
                switch (Type)
                {
                    case ActionType.Change:
                        Receiver.ImageName = (string)Value;
                        Receiver.LoadTexture();
                        
                        break;
                }
            }
            else if (Property.Equals("HasBecomeVisibleInAllColors"))
            {
                switch (Type)
                {
                    case ActionType.Change:
                        Receiver.HasBecomeVisibleInAllColors = (bool)Value;
                        Receiver.setVisibility(Colors.AllColors);
                        break;
                }
            }
        }
    }
}
