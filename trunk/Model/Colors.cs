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
    enum RawColor
    {
        Red = 1 << 0,
        Orange = 1 << 1,
        Yellow = 1 << 2,
        Green = 1 << 3,
        Blue = 1 << 4,
        Purple = 1 << 5
    };

    class Colors
    {
        // beware! fairly internal
        public int Bitstring { get; set;}

		/* Convenience Color Object */

        public static Colors AllColors = new Colors(true, true, true, true, true, true);
        public static Colors NoColors = new Colors(false, false, false, false, false, false);

		public static Colors RedColor = new Colors(true, false, false, false, false, false);
		public static Colors OrangeColor = new Colors(false, true, false, false, false, false);
		public static Colors YellowColor = new Colors(false, false, true, false, false, false);
		public static Colors GreenColor = new Colors(false, false, false, true, false, false);
		public static Colors BlueColor = new Colors(false, false, false, false, true, false);
		public static Colors PurpleColor = new Colors(false, false, false, false, false, true);
		
        public Colors(bool r, bool o, bool y, bool g, bool b, bool p)
        {
            Bitstring = 0;
            if (r)
                Bitstring |= (int)RawColor.Red;
            if (o)
                Bitstring |= (int)RawColor.Orange;
            if (y)
                Bitstring |= (int)RawColor.Yellow;
            if (g)
                Bitstring |= (int)RawColor.Green;
            if (b)
                Bitstring |= (int)RawColor.Blue;
            if (p)
                Bitstring |= (int)RawColor.Purple;
        }

		public Colors(int bitstring) {
			Bitstring = bitstring;
		}

        public static Colors ColorsFromJsonArray(ArrayList jsonArray)
        {
            Colors colors = new Colors(0);
            foreach (string color in jsonArray)
            {
                if (color.Equals("all")) {
                    return Colors.AllColors;
                }
                else if (color.Equals("no"))
                {
                    return Colors.NoColors;
                }
                else if (color.Equals("red"))
                {
                    colors.addRawColors(RawColor.Red);
                }
                else if (color.Equals("orange"))
                {
                    colors.addRawColors(RawColor.Orange);
                }
                else if (color.Equals("yellow"))
                {
                    colors.addRawColors(RawColor.Yellow);
                }
                else if (color.Equals("green"))
                {
                    colors.addRawColors(RawColor.Green);
                }
                else if (color.Equals("blue"))
                {
                    colors.addRawColors(RawColor.Blue);
                }
                else if (color.Equals("purple"))
                {
                    colors.addRawColors(RawColor.Purple);
                }
            }
            return colors;
        }

        // the player's viewing color is the receiver
        // the colors of the object to be seen is the parameter
        public bool contains(Colors c)
        {
            return (Bitstring & c.Bitstring) == Bitstring;
        }

		// Whether this represents just a singluar color like red, green, blue, etc
        public bool isSingularColor()
        {
            return (Bitstring == (int)RawColor.Red || Bitstring == (int)RawColor.Orange || Bitstring == (int)RawColor.Yellow || 
                    Bitstring == (int)RawColor.Green || Bitstring == (int)RawColor.Blue || Bitstring == (int)RawColor.Purple);
        }

		// Returns a new Colors object by combining components of this and 
		// another Colors object
		public Colors combine(Colors c) {
			return new Colors(Bitstring | c.Bitstring);
		}

        public void addRawColors(RawColor raw)
        {
            Bitstring |= (int) raw;
        }

        public Colors ForwardColor()
        {
            if (!isSingularColor())
            {
                return RedColor;
            }
            else
            {
                int next = (int)Bitstring << 1;
                if (next > 1 << 5)
                {
                    next = 1;
                }
                return new Colors(next);
            }
        }

        public Colors BackwardColor()
        {
            if (!isSingularColor())
            {
                return RedColor;
            }
            else
            {
                int next = (int)Bitstring >> 1;
                if (next <= 0)
                {
                    next = 1 << 5;
                }
                return new Colors(next);
            }
        }

        public override string ToString()
        {
            return string.Format("(Color {0})", Convert.ToString(this.Bitstring,2));
        }

        public Color SystemColor()
        {
            if (!isSingularColor())
            {
                throw new Exception("System color from multi-color");
            }
            else if (Bitstring == (int) RawColor.Red)
            {
                return new Color(226, 9, 21, 255);
            }
            else if (Bitstring == (int)RawColor.Orange)
            {
                return Color.Orange;
            }
            else if (Bitstring == (int)RawColor.Yellow)
            {
                return new Color(255,248,99,255);
            }
            else if (Bitstring == (int)RawColor.Green)
            {
                return Color.LightGreen;
            }
            else if (Bitstring == (int)RawColor.Blue)
            {
                return new Color(51,124,255,255);
            }
            else if (Bitstring == (int)RawColor.Purple)
            {
                return Color.BlueViolet;
            }
            throw new Exception("Color was invalid color value");
        }
    }
}
