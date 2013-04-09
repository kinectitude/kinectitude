//-----------------------------------------------------------------------
// <copyright file="KeyEvent.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;
using System.Windows.Forms;

namespace Kinectitude.Input
{
    public enum KeyState { Pressed, Released, Down }

    [Plugin("when key {Key} is {KeyState}", "Key Event")]
    public class KeyEvent : Event, IKeyChange
    {
        [PluginProperty("Key State", "State of the key. Down, Pressed or Released", KeyState.Down)]
        public KeyState KeyState { get; set; }

        [PluginProperty("Key", "Key to follow", Keys.Enter)]
        public Keys Key { get; set; }

        public override void OnInitialize()
        {
            GetManager<KeyboardManager>().RegisterIKeyChange(this);
        }
    }
}
