using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using SlimDX.DirectInput;
using Kinectitude.Core.Attributes;

namespace Kinectitude.DirectInput
{
    public enum KeyState { Pressed, Released, Down }

    [Plugin("Event for keys on the keyboard","")]
    public class KeyEvent : Event
    {
        [Plugin("State of the key","Down, Pressed or Released")]
        public KeyState KeyState { get; set; }
        [Plugin("Button to follow", "")]
        public Key Button { get; set; }

        public override void OnInitialize()
        {
            GetManager<KeyboardManager>().RegisterKeyEvent(this);
        }
    }
}
