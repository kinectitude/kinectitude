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

        private readonly List<KeyEvent> KeyPressed = new List<KeyEvent>();
        private readonly List<KeyEvent> KeyReleased = new List<KeyEvent>();
        private readonly List<KeyEvent> KeyDown = new List<KeyEvent>();

        public KeyboardManager()
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
                foreach (KeyEvent evt in KeyPressed.Where(input => input.Button == key)) evt.DoActions();
            }

            foreach (Key key in state.ReleasedKeys)
            {
                keysDown[(int)key] = false;
                foreach (KeyEvent evt in KeyReleased.Where(input => input.Button == key)) evt.DoActions();
            }

            for (int i = 0; i < keysDown.Length; i++)
            {
                if (keysDown[i])
                {
                    foreach (KeyEvent evt in KeyDown.Where(input => (int)input.Button == i)) evt.DoActions();
                }
            }

        }

        public void RegisterKeyEvent(KeyEvent keyEvent)
        {
            switch (keyEvent.KeyState)
            {
                case KeyState.Pressed:
                    KeyPressed.Add(keyEvent);
                    break;
                case KeyState.Down:
                    KeyDown.Add(keyEvent);
                    break;
                case KeyState.Released:
                    KeyReleased.Add(keyEvent);
                    break;
            }
        }

        public void RemoveKeyEvent(KeyEvent keyEvent)
        {
            switch (keyEvent.KeyState)
            {
                case KeyState.Pressed:
                    KeyPressed.Remove(keyEvent);
                    break;
                case KeyState.Down:
                    KeyDown.Remove(keyEvent);
                    break;
                case KeyState.Released:
                    KeyReleased.Remove(keyEvent);
                    break;
            }
        }
    }
}
