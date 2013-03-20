using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kinectitude.Input
{
    public class KeyChangeCallback : IKeyChange
    {
        public KeyState KeyState
        {
            get { return KeyState.Down; }
        }
        public Keys Key { get; set; }

        private readonly Action Call;

        public KeyChangeCallback(Action call) 
        { 
            Call = call;
        }

        public void DoActions() { Call(); }
    }
}
