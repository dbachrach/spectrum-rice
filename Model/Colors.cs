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
        private int Bitstring { get; set;}

		/* Convenience Color Object */

        // Color Collections
        public static Colors AllColors = new Colors(true, true, true, true, true, true);
        public static Colors NoColors = new Colors(false, false, false, false, false, false);
        public static Colors PrimaryColors = new Colors(true, false, true, false, true, false);
        public static Colors SecondaryColors = new Colors(false, true, false, true, false, true);


        // Singular Colors
		public static Colors PurpleColor = new Colors(true, false, false, false, false, false);
		public static Colors BlueColor = new Colors(false, true, false, false, false, false);
		public static Colors GreenColor = new Colors(false, false, true, false, false, false);
		public static Colors YellowColor = new Colors(false, false, false, true, false, false);
		public static Colors OrangeColor = new Colors(false, false, false, false, true, false);
		public static Colors RedColor = new Colors(false, false, false, false, false, true);

        

        public static List<string> ListOfColorsNames = new List<string>() { "purple", "blue", "green", "yellow", "orange", "red" };
        public static List<Colors> ListOfColors = new List<Colors>() { PurpleColor, BlueColor, GreenColor, YellowColor, OrangeColor, RedColor };
		
        /// <summary>
        /// Creates a Colors object from a set of flags each representing a color.
        /// </summary>
        public Colors(bool purple, bool blue, bool green, bool yellow, bool orange, bool red)
        {
            Bitstring = 0;
            if (purple)
                Bitstring |= (int)RawColor.Purple;
            if (blue)
                Bitstring |= (int)RawColor.Blue;
            if (green)
                Bitstring |= (int)RawColor.Green;
            if (yellow)
                Bitstring |= (int)RawColor.Yellow;
            if (orange)
                Bitstring |= (int)RawColor.Orange;
            if (red)
                Bitstring |= (int)RawColor.Red;
        }

        /// <summary>
        /// Creates a Colors object from a bit string.
        /// </summary>
        /// <param name="bitstring">Bit string with length 6. Each bit represents a color. 
        /// Colors go from MSB to LSB as Purple, Blue, Green, Yellow, Orange, Red.
        /// Use RawColor to specify colors.</param>
        public Colors(int bitstring)
        {
            Bitstring = bitstring;
        }

        /// <summary>
        /// Given the set of colors inColors, returns an index of THIS color in that set.
        /// For instance, if inColors = PGOR, then if THIS color is Orange, returns 2. 
        /// If THIS color is Red, returns 3.
        /// </summary>
        public int IndexIn(Colors inColors)
        {
            int index = 0;
            if (!IsSingularColor())
            {
                throw new Exception("Should not call IndexIn() on a non-singular color");
            }

            foreach (Colors c in ListOfColors)
            {
                if (this.Equals(c))
                {
                    return index;
                }
                else if (inColors.Contains(c))
                {
                    index++;
                }
            }

            throw new Exception("Finding index threw an error");
        }

        /// <summary>
        /// Creates a Colors object from an array of named colors.
        /// </summary>
        /// <param name="jsonArray">Array of strings with colors being named as:
        /// "purple", "blue", "green", "yellow", "orange", or "red".</param>
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

                for (int i = 0; i < ListOfColors.Count(); i++)
                {
                    if (color.Equals(ListOfColorsNames[i]))
                    {
                        colors.Combine(ListOfColors[i]);
                    }
                }
            }
            return colors;
        }

        /// <summary>
        /// Returns whether two Colors objects are equal.
        /// Equality means that the two objects represent the exact same set of colors.
        /// </summary>
        public bool Equals(Colors c)
        {
            //return ( (Bitstring & c.Bitstring) == Bitstring && (Bitstring & c.Bitstring) == c.Bitstring);
            return this.Bitstring == c.Bitstring;
        }

        // the player's viewing color is the receiver
        // the colors of the object to be seen is the parameter

        /// <summary>
        /// Returns whether c is contained in the receiver.
        /// For example: the player's viewing color is the receiver, 
        /// and the color of the object tested to be seen is the parameter
        /// </summary>
        /// <param name="c">The color to be tested for containment in the reciever</param>
        public bool Contains(Colors c)
        {
            return (Bitstring & c.Bitstring) == c.Bitstring; //== Bitstring;
        }

        /// <summary>
        /// Whether this represents just a singular color: purple, blue, green, yellow, orange, or red
        /// </summary>
        public bool IsSingularColor()
        {
            foreach (Colors c in ListOfColors)
            {
                if (this.Equals(c))
                {
                    return true;
                }
            }
            return false;
            //return (Bitstring == (int)RawColor.Red || Bitstring == (int)RawColor.Orange || Bitstring == (int)RawColor.Yellow || 
            //        Bitstring == (int)RawColor.Green || Bitstring == (int)RawColor.Blue || Bitstring == (int)RawColor.Purple);
        }

        /// <summary>
        /// Creates a new Colors object by adding all of c's colors to the reciever
        /// </summary>
		public Colors ColorsByCombiningWith(Colors c) 
        {
			return new Colors(Bitstring | c.Bitstring);
		}

        /// <summary>
        /// Check if this Colors contains at least one color contained by the other colors
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool SharesAColorWith(Colors c)
        {
            return (Bitstring & c.Bitstring) != 0;
        }

        /// <summary>
        /// Creates a new Colors object by removing all of c's colors from the reciever
        /// 
        /// </summary>
        public Colors ColorsByDifferencingWith(Colors c)
        {
            return new Colors(Bitstring - (Bitstring & c.Bitstring));
        }

        /// <summary>
        /// Mutates the reciever to now include all colors in c
        /// </summary>
        public void Combine(Colors c)
        {
            Bitstring = Bitstring | c.Bitstring;
        }

        /// <summary>
        /// Returns the next forward color (does not mutate). In terms of the bitstring, this moves from LSB to MSB 1 digit.
        /// For example, if the reciever is currently red, an orange Colors object will be returned.
        /// This wraps, so a purple reciever will return a red colors object.
        /// 
        /// </summary>
        /// <exception cref="Exception">Cannot forward a non-singular color</exception>
        public Colors ForwardColor()
        {
            if (!IsSingularColor())
            {
                throw new Exception("Cannot forward color on a non-singular color");
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

        /// <summary>
        /// Returns the backward color (does not mutate). In terms of the bitstring, this moves from MSB to LSB 1 digit.
        /// For example, if the reciever is currently purple, a blue Colors object will be returned.
        /// This wraps, so a red reciever will return a purple colors object.
        /// 
        /// </summary>
        /// <exception cref="Exception">Cannot backward a non-singular color</exception>
        public Colors BackwardColor()
        {
            if (!IsSingularColor())
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

        /// <summary>
        /// Returns a string representing the color. The first letter of each color in the reciever will
        /// be returned capitalized. The order is purple, blue, green, yellow, orange, red.
        /// </summary>
        public override string ToString()
        {
            //return string.Format("(Color {0})", Convert.ToString(this.Bitstring,2));
            string str = "";

            if (this.Contains(PurpleColor))
            {
                str += "P";
            }
            if (this.Contains(BlueColor))
            {
                str += "B";
            }
            if (this.Contains(GreenColor))
            {
                str += "G";
            }
            if (this.Contains(YellowColor))
            {
                str += "Y";
            }
            if (this.Contains(OrangeColor))
            {
                str += "O";
            }
            if (this.Contains(RedColor))
            {
                str += "R";
            }
            return str;
        }

        /// <summary>
        /// The number of colors in this object.
        /// </summary>
        public int Count()
        {
            int cnt = 0;

            foreach (Colors c in ListOfColors)
            {
                if (this.Contains(c))
                {
                    cnt++;
                }
            }

           return cnt;
        }

        /// <summary>
        /// Returns a system Color object to draw each individual Colors singular color.
        /// </summary>
        public Color SystemColor()
        {
            if (!IsSingularColor())
            {
                return new Color(226, 9, 21, 255);
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

        /// <summary>
        /// Returns a new colors object that is the result of combining the reciever with c.
        /// If these two colors cannot be returned, returns null.
        /// </summary>
        public Colors ColorByMixingWith(Colors c)
        {
            if (this.Equals(Colors.RedColor))
            {
                if (c.Equals(Colors.BlueColor))
                {
                    return PurpleColor;
                }
                else if (c.Equals(Colors.YellowColor))
                {
                    return OrangeColor;
                }
            }
            else if (this.Equals(Colors.YellowColor))
            {
                if (c.Equals(Colors.BlueColor))
                {
                    return GreenColor;
                }
                else if (c.Equals(Colors.RedColor))
                {
                    return OrangeColor;
                }
            }
            else if (this.Equals(Colors.BlueColor))
            {
                if (c.Equals(Colors.RedColor))
                {
                    return PurpleColor;
                }
                else if (c.Equals(Colors.YellowColor))
                {
                    return GreenColor;
                }
            }
            return null;
        }
    }
}
