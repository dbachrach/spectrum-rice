using System;

namespace Spectrum
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SpectrumGame game = new SpectrumGame())
            {
                game.Run();
            }
        }
    }
}

