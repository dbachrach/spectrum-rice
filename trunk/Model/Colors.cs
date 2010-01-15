using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spectrum.Model
{
    enum RawColors
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
        public int Bitstring { get; set; }

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
                Bitstring |= (int)RawColors.Red;
            if (o)
                Bitstring |= (int)RawColors.Orange;
            if (y)
                Bitstring |= (int)RawColors.Yellow;
            if (g)
                Bitstring |= (int)RawColors.Green;
            if (b)
                Bitstring |= (int)RawColors.Blue;
            if (p)
                Bitstring |= (int)RawColors.Purple;
        }

		public Colors(int bitstring) {
			Bitstring = bitstring;
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
            return (Bitstring == (int)RawColors.Red || Bitstring == (int)RawColors.Orange || Bitstring == (int)RawColors.Yellow || 
                    Bitstring == (int)RawColors.Green || Bitstring == (int)RawColors.Blue || Bitstring == (int)RawColors.Purple);
        }

		// Returns a new Colors object by combining components of this and 
		// another Colors object
		public Colors combine(Colors c) {
			return new Colors(Bitstring | c.Bitstring);
		}

  
    }
}
