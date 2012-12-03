using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using SlimDX.DirectInput;
using SlimDX;
using Kinectitude.Core.Attributes;

namespace Kinectitude.DirectInput
{
    [Plugin("Keyboard Manager", "")]
    public class KeyboardManager : Manager<Component>
    {
        private static Keyboard keyboard = null;

        private bool[] keysDown = new bool[Enum.GetNames(typeof(Key)).Length];

        private readonly List<IKeyChange> KeyPressed = new List<IKeyChange>();
        private readonly List<IKeyChange> KeyReleased = new List<IKeyChange>();
        private readonly List<IKeyChange> KeyDown = new List<IKeyChange>();

        public KeyboardManager()
        {
            if (keyboard == null)
            {
                DirectInputService service = GetService<DirectInputService>();
                keyboard = new Keyboard(service.DirectInput);
                service.InitDevice<Keyboard>(keyboard);
            }
            for (int i = 0; i < keysDown.Length; i++)
            {
                keysDown[i] = false;
            }
        }

        public override void OnUpdate(float frameDelta)
        {
            if (keyboard.Acquire().IsFailure || keyboard.Poll().IsFailure) return;
            KeyboardState state = keyboard.GetCurrentState();
            if (Result.Last.IsFailure) return;

            foreach (Key key in state.PressedKeys)
            {
                foreach (IKeyChange evt in KeyDown.Where(input => input.Button == key)) evt.DoActions();
                if (keysDown[(int)key]) continue;
                foreach (IKeyChange evt in KeyPressed.Where(input => input.Button == key)) evt.DoActions();
                keysDown[(int)key] = true;
            }

            foreach (Key key in state.ReleasedKeys)
            {
                if (!keysDown[(int)key]) continue;
                foreach (IKeyChange evt in KeyReleased.Where(input => input.Button == key)) evt.DoActions();
                keysDown[(int)key] = false;
            }

            foreach (Component child in Children){child.OnUpdate(frameDelta);}
        }

        public void RegisterIKeyChange(IKeyChange keyEvent)
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

        public void RemoveIKeyChange(IKeyChange keyEvent)
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
