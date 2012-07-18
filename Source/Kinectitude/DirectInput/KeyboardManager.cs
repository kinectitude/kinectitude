using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using SlimDX.DirectInput;
using SlimDX;

namespace Kinectitude.DirectInput
{
    public class KeyboardManager : Manager<Component>
    {
        private static Keyboard keyboard = null;

        private bool[] keysDown = new bool[Enum.GetNames(typeof(Key)).Length];

        KeyboardManager()
        {
            if (keyboard == null)
            {
                DirectInputService service = GetService<DirectInputService>();
                keyboard = new Keyboard(service.DirectInput);
                service.InitDevice<Keyboard>(keyboard);
            }
        }

        public override void OnUpdate(float frameDelta)
        {
            if (keyboard.Acquire().IsFailure || keyboard.Poll().IsFailure) return;
            KeyboardState state = keyboard.GetCurrentState();
            if (Result.Last.IsFailure) return;



            foreach (Key key in state.PressedKeys)
            {

                keysDown[(int)key] = true;
            }

            foreach (Key key in state.ReleasedKeys)
            {

                keysDown[(int)key] = false;
            }
        }
    }
}
