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

        /* Must set this externally once per cycle */
        public static KeyboardState Keyboard { get; set; }
        public static GamePadState Gamepad { get; set; }
        public static KeyboardState PreviousKeyboard { get; set; }
        public static GamePadState PreviousGamepad { get; set; }

        public static bool UserInputPress(Keys key, Buttons button)
        {
            return (Keyboard.IsKeyDown(key) && !PreviousKeyboard.IsKeyDown(key)) || (Gamepad.IsButtonDown(button) && !PreviousGamepad.IsButtonDown(button));
        }

        public static bool UserInputHold(Keys key, Buttons button)
        {
            return (Keyboard.IsKeyDown(key)) || (Gamepad.IsButtonDown(button));
        }

        public static bool UserInputRelease(Keys key, Buttons button)
        {
            return (!Keyboard.IsKeyDown(key) && PreviousKeyboard.IsKeyDown(key)) || (!Gamepad.IsButtonDown(button) && PreviousGamepad.IsButtonDown(button));
        }
    }

    
}
