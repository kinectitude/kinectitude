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

        private readonly List<IKeyChange> keyPressed = new List<IKeyChange>();
        private readonly List<IKeyChange> keyReleased = new List<IKeyChange>();
        private readonly List<IKeyChange> keyDown = new List<IKeyChange>();
        //private readonly bool[] keysDown = new bool[255];
        private readonly Dictionary<Keys, bool> keyStates = new Dictionary<Keys, bool>();
        private InputService inputService;

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
            foreach (var evt in keyPressed.Where(x => x.Key == key))
            {
                evt.DoActions();
            }

            keyStates[key] = true;
        }

        private void OnKeyUp(Keys key, int code)
        {
            foreach (var evt in keyReleased.Where(x => x.Key == key))
            {
                evt.DoActions();
            }

            keyStates[key] = false;
        }

        public override void OnUpdate(float frameDelta)
        {
            foreach (Component child in Children)
            {
                child.OnUpdate(frameDelta);
            }

            //foreach (Keys value in KeyValues)
            //{
            //    if (keysDown[(int)value])
            //    {
            //        foreach (var evt in keyDown.Where(x => x.Key == value))
            //        {
            //            evt.DoActions();
            //        }
            //    }
            //}

            foreach (var state in keyStates.Where(x => x.Value == true))
            {
                foreach (var evt in keyDown.Where(x => x.Key == state.Key))
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
                    keyPressed.Add(keyEvent);
                    break;
                case KeyState.Down:
                    keyDown.Add(keyEvent);
                    break;
                case KeyState.Released:
                    keyReleased.Add(keyEvent);
                    break;
            }
        }

        public void RemoveIKeyChange(IKeyChange keyEvent)
        {
            switch (keyEvent.KeyState)
            {
                case KeyState.Pressed:
                    keyPressed.Remove(keyEvent);
                    break;
                case KeyState.Down:
                    keyDown.Remove(keyEvent);
                    break;
                case KeyState.Released:
                    keyReleased.Remove(keyEvent);
                    break;
            }
        }
    }
}
