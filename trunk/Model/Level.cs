using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spectrum.Model
{
    class Level
    {
        public int Id 
        {
            get { return id; }
            set { id = value; }
        }
        private int id;

        public int Number
        {
            get { return number; }
            set { number = value; }
        }
        private int number;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string name;

    }
}
