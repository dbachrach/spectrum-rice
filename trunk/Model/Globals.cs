using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spectrum.Model
{
    /**
     * Declares constant string names as static globals
     */
    class Globals
    {
        public static string ColorsProperty = "colors";
        public static string PlayerTangibilityProperty = "player-tangibility";
        public static string ImageProperty = "image";

        public static string ChangeAction = "change";
        public static string IncrementAction = "increment";
        public static string DecrementAction = "decrement";
        public static string AddColorsAction = "add-colors";
        public static string RemoveColorsAction = "remove-colors";

        public static string WinSpecial = "win";
        public static string LoseSpecial = "lose";

        public static string ResumeMenuItem = "Resume";
        public static string RestartMenuItem = "Restart";
        public static string SettingsMenuItem = "Settings";
        public static string ExitMenuItem = "Exit";
    }
}
