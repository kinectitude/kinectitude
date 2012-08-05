using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;

namespace Kinectitude.DirectInput
{
    public class KeyChangeCallback : IKeyChange
    {
        public KeyState KeyState { get; private set; }
        public Key Button { get; set; }

        private readonly Action Call;

        public KeyChangeCallback(Action call) 
        { 
            Call = call;
            KeyState = KeyState.Down;
        }

        public void DoActions() { Call(); }
    }
}
