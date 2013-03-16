using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kinectitude.Input
{
    public class KeyChangeCallback : IKeyChange
    {
        public KeyState KeyState { get; private set; }
        public Keys Key { get; set; }

        private readonly Action Call;

        public KeyChangeCallback(Action call) 
        { 
            Call = call;
            KeyState = KeyState.Down;
        }

        public void DoActions() { Call(); }
    }
}
