using System;
using System.Collections;
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
	enum ActionType { Change, Increment, Decrement, AddColors, RemoveColors, Range }
	
    class EventAction
    {
        private int RepeatIndef = -999;

        public GameObject Receiver { get; set; }
		public string Property { get; set; }
		public ActionType Type { get; set; }
		public object Value { get; set; }
		public bool Animated { get; set; }
		public float AnimationDuration { get; set; }
		public float Delay { get; set; }
		public bool Repeats { get; set; }
        public int RepeatCount { get; set; }
		public float RepeatDelay { get; set; }
        public string Special { get; set; }
        public double LaunchTime { get; set;  }

		/* Default constructor. */
        public EventAction()
        {
			Animated = false;
			AnimationDuration = 1;
			Delay = 0;
			Repeats = false;
            RepeatCount = RepeatIndef;
			RepeatDelay = 1;
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
            else if (str.Equals(Globals.RangeAction))
            {
                return ActionType.Range;
            }
            else
            {
                throw new Exception("Type for action was not an action type: " + str);
            }
        }

        public virtual bool HandleDelay(List<EventAction> deferFuture, double curMs)
        {
            bool update = (Delay > 0);
            if (update)
            {
                LaunchTime = curMs + Delay;
                Delay = 0;
                Console.WriteLine("Adding {0} to futures at launch time {1}", this, LaunchTime);
                deferFuture.Add(this);
            }

            return update;
        }

        public virtual void HandleRepeats(List<EventAction> deferFuture, double curMs)
        {
            if (Repeats)
            {
                if (RepeatCount == RepeatIndef || RepeatCount > 0)
                {
                    this.LaunchTime = curMs + this.RepeatDelay;
                    if (RepeatCount != RepeatIndef)
                    {
                        this.RepeatCount--;
                    }
                    deferFuture.Add(this);
                }
            }
        }

        public void Execute(List<EventAction> deferFuture, double curMs)
        {
            if (!HandleDelay(deferFuture, curMs))
            {
                // do this EventAction's specific action
                InnerExecute(deferFuture, curMs);

                HandleRepeats(deferFuture, curMs);
            }
        }

        public virtual void InnerExecute(List<EventAction> deferFuture, double curMs)
        {
            /* TODO: All execution properties */
            if (Special != null && !Special.Equals(""))
            {
                if (Special.Equals(Globals.WinSpecial))
                {
                    Console.WriteLine("WIN");
                    Player p = (Player)this.Receiver;
                    p.WinLevel();
                }
                else if (Special.Equals(Globals.LoseSpecial))
                {
                    Console.WriteLine("LOSE");
                    Player p = (Player)this.Receiver;
                    p.LoseLevel();
                }
            }
            else if(Property.Equals(Globals.PositionProperty))
            {
                switch (Type)
                {
                    case ActionType.Change:
                        ArrayList vals = (ArrayList)Value;
                        Vector2 vec = new Vector2((float)((double)vals[0]), (float)((double)vals[1]));
                        Receiver.body.Position += vec;
                        break;
                }
            }
            else if (Property.Equals(Globals.ColorsProperty))
            {
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
            else if (Property.Equals(Globals.ImageProperty))
            {
                switch (Type)
                {
                    case ActionType.Change:
                        Receiver.ImageName = (string)Value;
                        Receiver.LoadTexture();

                        break;
                }
            }
            else if (Property.Equals("force"))
            {
                switch (Type)
                {
                    case ActionType.Change:
                        ArrayList vals = (ArrayList)Value;
                        Vector2 vec = new Vector2((float)((double)vals[0]), (float)((double)vals[1]));
                        Console.WriteLine("Vector behav: {0}", vec);
                        Receiver.body.ApplyForce(vec);

                        break;
                }
            }
            else if (Property.Equals("whiteness"))
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
