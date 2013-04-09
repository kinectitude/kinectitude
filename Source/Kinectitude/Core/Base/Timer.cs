//-----------------------------------------------------------------------
// <copyright file="Timer.cs" company="Kinectitude">
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
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Base
{
    internal sealed class Timer
    {
        internal string Name { get; private set; }
        internal bool Recurring { get; private set; }

        private float time;
        private readonly float initTime;

        internal Timer(string name, float time, bool recurring)
        {
            Name = name;
            initTime = this.time = time;
            Recurring = recurring;
        }

        internal bool tick(float duration)
        {
            time -= duration;
            bool trigger = time <= 0;
            if (Recurring && trigger)
            {
                time = initTime;
            }
            return trigger;
        }
    }
}
