using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;

namespace CrashBandicoot
{
    enum Button
    {
        Left,
        Right,
        Down,
        Up,
        Attack,
        Enter,
    }

    static class InputManager
    {
        private static bool joypadInput;
        private static int playerIndex = 0;

        static InputManager()
        {
            joypadInput = false;
        }

        public static void Update()
        {
            string[] joysticks = Game.Window.Joysticks;

            for (int i = 0; i < joysticks.Length; i++)
            {
                if (joysticks[i] != null && joysticks[i] != "Unmapped Controller")
                {
                    if (!joypadInput)
                    {
                        playerIndex = i;
                        joypadInput = true;
                        return;
                    }
                    else
                    {
                        return;
                    }
                }

                joypadInput = false;
            }
        }
        
        public static bool GetButton(Button button)
        {
            if (!joypadInput)
            {
                switch (button)
                {
                    case Button.Left:
                        return (Game.Window.GetKey(KeyCode.Left));
                    case Button.Right:
                        return (Game.Window.GetKey(KeyCode.Right));
                    case Button.Down:
                        return (Game.Window.GetKey(KeyCode.Down));
                    case Button.Up:
                        return (Game.Window.GetKey(KeyCode.Up));
                    case Button.Attack:
                        return (Game.Window.GetKey(KeyCode.Space));
                    case Button.Enter:
                        return (Game.Window.GetKey(KeyCode.Return));
                    default:
                        return false;
                }
            }
            else
            {
                switch (button)
                {
                    case Button.Left:
                        return (Game.Window.JoystickAxisLeft(playerIndex).X < -0.5f || Game.Window.JoystickLeft(playerIndex));
                    case Button.Right:
                        return (Game.Window.JoystickAxisLeft(playerIndex).X > 0.5f || Game.Window.JoystickRight(playerIndex));
                    case Button.Down:
                        return (Game.Window.JoystickB(playerIndex) || Game.Window.JoystickDown(playerIndex));
                    case Button.Up:
                        return (Game.Window.JoystickA(playerIndex) || Game.Window.JoystickUp(playerIndex));
                    case Button.Attack:
                        return (Game.Window.JoystickX(playerIndex));
                    case Button.Enter:
                        return (Game.Window.JoystickA(playerIndex) || Game.Window.JoystickStart(playerIndex)); 
                    default:
                        return false;
                }
            }
        }
    }
}