//-----------------------------------------------------------------------
// <copyright file="TimerEvt.cs" company="Kinectitude">
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

namespace Kinectitude.Core.Base
{

    internal enum EType { Create, Pause, Resume }

    internal sealed class TimerEvt
    {
        internal string Name { get; private set; }
        internal EType Type { get; private set; }
        internal Timer Timer { get; private set; }

        internal TimerEvt(EType type, string name, Timer timer = null)
        {
            Name = name;
            Type = type;
            Timer = timer;
        }
    }
}
