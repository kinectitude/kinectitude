﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    [Plugin("Pause timers with a given name", "")]
    internal class PauseTimersAction : Action
    {
        [Plugin("Name of the timers to pause", "")]
        public IExpressionReader Name { get; set; }

        public override void Run()
        {
            Event.Entity.Scene.PauseTimers(Name.GetValue());
        }
    }
}