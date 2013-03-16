using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;
using System.Windows.Forms;

namespace Kinectitude.Input
{
    [Plugin("Keyboard Manager", "")]
    public class KeyboardManager : Manager<Component>
    {
        private static readonly Array KeyValues = Enum.GetValues(typeof(Keys));
        //private static Keyboard keyboard = null;

        //private bool[] keysDown = new bool[Enum.GetNames(typeof(Key)).Length];

        private bool[] keysDown = new bool[KeyValues.Length];

        private readonly List<IKeyChange> KeyPressed = new List<IKeyChange>();
        private readonly List<IKeyChange> KeyReleased = new List<IKeyChange>();
        private readonly List<IKeyChange> KeyDown = new List<IKeyChange>();
        private InputService inputService;

        public KeyboardManager()
        {
            //if (keyboard == null)
            //{
            //    InputService service = GetService<InputService>();
            //    keyboard = new Keyboard(service.DirectInput);
            //    service.InitDevice<Keyboard>(keyboard);
            //}
            //for (int i = 0; i < keysDown.Length; i++)
            //{
            //    keysDown[i] = false;
            //}
        }

        protected override void OnStart()
        {
            if (null == inputService)
            {
                inputService = GetService<InputService>();
                inputService.KeyDown += OnKeyDown;
                inputService.KeyUp += OnKeyUp;
            }
        }

        protected override void OnStop()
        {
            inputService.KeyDown -= OnKeyDown;
            inputService.KeyUp -= OnKeyUp;
        }

        private void OnKeyDown(Keys key, int code)
        {
            foreach (var evt in KeyDown.Where(x => x.Key == key))
            {
                evt.DoActions();
            }

            keysDown[code] = true;
        }

        private void OnKeyUp(Keys key, int code)
        {
            foreach (var evt in KeyReleased.Where(x => x.Key == key))
            {
                evt.DoActions();
            }

            keysDown[code] = false;
        }

        public override void OnUpdate(float frameDelta)
        {
            //if (keyboard.Acquire().IsFailure || keyboard.Poll().IsFailure) return;
            //KeyboardState state = keyboard.GetCurrentState();
            //if (Result.Last.IsFailure) return;

            //foreach (Key key in state.PressedKeys)
            //{
            //    foreach (IKeyChange evt in KeyDown.Where(input => input.Button == key)) evt.DoActions();
            //    if (keysDown[(int)key]) continue;
            //    foreach (IKeyChange evt in KeyPressed.Where(input => input.Button == key)) evt.DoActions();
            //    keysDown[(int)key] = true;
            //}

            //foreach (Key key in state.ReleasedKeys)
            //{
            //    if (!keysDown[(int)key]) continue;
            //    foreach (IKeyChange evt in KeyReleased.Where(input => input.Button == key)) evt.DoActions();
            //    keysDown[(int)key] = false;
            //}

            //foreach (Component child in Children){child.OnUpdate(frameDelta);}

            foreach (Keys value in KeyValues)
            {
                foreach (var evt in KeyPressed.Where(x => x.Key == value))
                {
                    evt.DoActions();
                }
            }
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
