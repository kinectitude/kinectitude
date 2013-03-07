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

    [Plugin("when key {Button} is {KeyState}", "Key Event")]
    public class KeyEvent : Event, IKeyChange
    {
        [PluginProperty("Key State", "State of the key. Down, Pressed or Released")]
        public KeyState KeyState { get; set; }

        [PluginProperty("Button", "Button to follow")]
        public Key Button { get; set; }

        public override void OnInitialize()
        {
            GetManager<KeyboardManager>().RegisterIKeyChange(this);
        }
    }
}
