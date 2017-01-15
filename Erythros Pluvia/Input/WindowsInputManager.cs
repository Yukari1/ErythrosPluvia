using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Erythros_Pluvia.Input
{

    #region Delegates

    public delegate void MouseButtonCommand(int x, int y, GameTime time);

    public delegate void MouseScrollCommand(int currentScrollValue, int previousScrollValue, GameTime time);

    #endregion

    public class WindowsInputManager : InputManager
    {

        #region Fields

        // an associative array of commands bound to key presses
        protected Dictionary<Keys, Command> keyPressBindings;

        // an associative array of commands bound to key releases
        protected Dictionary<Keys, Command> keyReleaseBindings;

        // command bound to left mouse button click
        protected MouseButtonCommand leftButtonClickBinding;

        // command bound to the right mouse button click
        protected MouseButtonCommand rightButtonClickBinding;

        // command bound to the middle mouse button click
        protected MouseButtonCommand middleButtonClickBinding;

        // command bound to left mouse button release
        protected MouseButtonCommand leftButtonReleaseBinding;

        // command bound to right mouse button release
        protected MouseButtonCommand rightButtonReleaseBinding;

        // command bound to middle mouse button release
        protected MouseButtonCommand middleButtonReleaseBinding;

        // command bound to the mouse scroll wheel
        protected MouseScrollCommand scrollBinding;

        // for storing the previous state to detect if a button was released
        protected KeyboardState oldKeyboardState;

        // for storing the previous mouse scroll value
        protected MouseState oldMouseState;

        #endregion

        #region Methods

        public WindowsInputManager()
        {
            keyPressBindings = new Dictionary<Keys, Command>();
            keyReleaseBindings = new Dictionary<Keys, Command>();
            oldKeyboardState = Keyboard.GetState();

            leftButtonClickBinding = null;
            rightButtonClickBinding = null;
            middleButtonClickBinding = null;

            leftButtonReleaseBinding = null;
            rightButtonReleaseBinding = null;
            middleButtonReleaseBinding = null;

            scrollBinding = null;

            oldMouseState = Mouse.GetState();
        }

        public void executeCommands(GameTime time)
        {
            _executeKeyboardCommands(time);

            _executeMouseCommands(time);
        }

        public void addKeyPressBinding(Keys key, Command command)
        {
            keyPressBindings.Add(key, command);
        }

        public void addKeyReleaseBinding(Keys key, Command command)
        {
            keyReleaseBindings.Add(key, command);
        }

        public void addLeftMouseButtonClickBinding(MouseButtonCommand command)
        {
            leftButtonClickBinding = command;
        }

        public void addRightMouseButtonClickBinding(MouseButtonCommand command)
        {
            rightButtonClickBinding = command;
        }

        public void addMiddleMouseButtonClickBinding(MouseButtonCommand command)
        {
            middleButtonClickBinding = command;
        }

        public void addMouseScrollBinding(MouseScrollCommand command)
        {
            scrollBinding = command;
        }

        private void _executeKeyboardCommands(GameTime time)
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
                if (oldKeyboardState.IsKeyDown(key) && keyboard.IsKeyUp(key))
                {
                    keyReleaseBindings[key](time);
                }
            }
            
            // store the current state as the old state in preparation for the next update
            oldKeyboardState = keyboard;
        }

        private void _executeMouseCommands(GameTime time)
        {
            // get the mouse state and check for mouse button clicks
            MouseState mouse = Mouse.GetState();

            // if there's a left mouse click binding, check for a left mouse click and execute the command
            if (leftButtonClickBinding != null)
            {
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    leftButtonClickBinding(mouse.X, mouse.Y, time);
                }
            }

            // if there's a left mouse release binding, check for a left mouse release and execute the command
            if (leftButtonReleaseBinding != null)
            {
                if (oldMouseState.LeftButton == ButtonState.Pressed && mouse.LeftButton == ButtonState.Released)
                {
                    leftButtonReleaseBinding(mouse.X, mouse.Y, time);
                }
            }

            // if there's a right mouse click binding, check for a right mouse click and execute the command
            if (rightButtonClickBinding != null)
            {
                if (mouse.RightButton == ButtonState.Pressed)
                {
                    rightButtonClickBinding(mouse.X, mouse.Y, time);
                }
            }

            // if there's a right mouse release binding, check for a right mouse release and execute the command
            if (rightButtonReleaseBinding != null)
            {
                if (oldMouseState.LeftButton == ButtonState.Pressed && mouse.LeftButton == ButtonState.Released)
                {
                    rightButtonReleaseBinding(mouse.X, mouse.Y, time);
                }
            }

            // if there's a middle mouse click binding, check for a middle mouse click and execute the command
            if (middleButtonClickBinding != null)
            {
                if (mouse.MiddleButton == ButtonState.Pressed)
                {
                    middleButtonClickBinding(mouse.X, mouse.Y, time);
                }
            }

            // if there's a middle mouse release binding, check for a middle mouse release and execute the command
            if (middleButtonReleaseBinding != null)
            {
                if (oldMouseState.MiddleButton == ButtonState.Pressed && mouse.MiddleButton == ButtonState.Released)
                {
                    middleButtonReleaseBinding(mouse.X, mouse.Y, time);
                }
            }

            // if there's a mouse scroll binding, check for a delta in the scroll wheel value and execute the command
            if (scrollBinding != null)
            {
                if (oldMouseState.ScrollWheelValue != mouse.ScrollWheelValue)
                {
                    scrollBinding(oldMouseState.ScrollWheelValue, mouse.ScrollWheelValue, time);
                }
            }

            // store the current scroll value as the old scroll value in preparation for the next update
            oldMouseState = mouse;
        }
    }

    #endregion
}
