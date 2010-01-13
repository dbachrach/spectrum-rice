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

        public static Colors AllColors = new Colors(true, true, true, true, true, true);
        public static Colors NoColors = new Colors(false, false, false, false, false, false);

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

        // the player's viewing color is the receiver
        // the colors of the object to be seen is the parameter
        public bool contains(Colors c)
        {
            return (Bitstring & c.Bitstring) == Bitstring;
        }

        public bool isSingularColor()
        {

        }

  
    }
}
