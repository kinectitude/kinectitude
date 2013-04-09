//-----------------------------------------------------------------------
// <copyright file="ResumeTimersAction.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    [Plugin("resume timers {Name}", "Resume timers")]
    internal sealed class ResumeTimersAction : Action
    {
        [PluginProperty("Name", "Name of the timers to resume")]
        public ValueReader Name { get; set; }

        public override void Run()
        {
            Event.Entity.Scene.ResumeTimers(Name);
        }
    }
}
