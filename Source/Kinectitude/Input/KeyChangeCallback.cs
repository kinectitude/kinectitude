//-----------------------------------------------------------------------
// <copyright file="KeyChangeCallback.cs" company="Kinectitude">
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
