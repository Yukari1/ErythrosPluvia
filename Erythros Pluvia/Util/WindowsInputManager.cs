using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Erythros_Pluvia.Util
{

    public class WindowsInputManager : InputManager
    {

        #region Fields

        // an associative array of commands bound to key presses
        protected Dictionary<Keys, Command> keyPressBindings;

        // an associative array of commands bound to key releases
        protected Dictionary<Keys, Command> keyReleaseBindings;

        // for storing the previous state to detect if a button was released
        protected KeyboardState oldState;

        #endregion

        #region Methods

        public WindowsInputManager()
        {
            keyPressBindings = new Dictionary<Keys, Command>();
            keyReleaseBindings = new Dictionary<Keys, Command>();
            oldState = Keyboard.GetState();
        }

        public void executeCommands(GameTime time)
        {
            KeyboardState keyboard = Keyboard.GetState();

            // check each key press binding and execute the action if it's pressed
            foreach (Keys key in keyPressBindings.Keys)
            {
                if (keyboard.IsKeyDown(key))
                {
                    keyPressBindings[key](time);
                }
            }

            // check each key release binding and execute the action if it's released
            foreach (Keys key in keyReleaseBindings.Keys)
            {
                if (oldState.IsKeyDown(key) && keyboard.IsKeyUp(key))
                {
                    keyReleaseBindings[key](time);
                }
            }

            // store the current state as the old state in preparation for the next update
            oldState = keyboard;
        }

        public void addKeyPressBinding(Keys key, Command command)
        {
            keyPressBindings.Add(key, command);
        }

        public void addKeyReleaseBinding(Keys key, Command command)
        {
            keyReleaseBindings.Add(key, command);
        }
    }

    #endregion
}
