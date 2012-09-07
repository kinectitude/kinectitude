﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    [Plugin("Resume timers {Name}", "")]
    internal class ResumeTimersAction : Action
    {
        [Plugin("Name", "Name of the timers to resume")]
        public IExpressionReader Name { get; set; }

        public override void Run()
        {
            Event.Entity.Scene.ResumeTimers(Name.GetValue());
        }
    }
}
