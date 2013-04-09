//-----------------------------------------------------------------------
// <copyright file="IKeyChange.cs" company="Kinectitude">
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
    public interface IKeyChange
    {
        KeyState KeyState { get; }
        Keys Key { get; set; }
        void DoActions();
    }
}
