using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Erythros_Pluvia.Util
{
    class WindowsInputManager
    {
        // an associative array of commands bound to key presses
        protected Dictionary<Key, Command> keyPressBindings;

        // an associative array of commands bound to key releases
        protected Dictionary<Key, Command> keyReleaseBindings;

        public void executeCommands()
        {
            // check each key press binding and execute the action if it's pressed
            foreach (Key key in keyPressBindings)
            {
                if (Keyboard.IsKeyDown(key))
                {
                    keyPressBindings[key].execute();
                }
            }

            // check each key release binding and execute the action if it's released
            foreach (Key key in keyReleaseBindings)
            {
                if (Keyboard.IsKeyUp(key))
                {
                    keyReleaseBindings[key].execute();
                }
            }
        }
    }
}
