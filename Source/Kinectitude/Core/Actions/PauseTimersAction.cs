using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    [Plugin("pause timers {Name}", "Pause timers")]
    internal sealed class PauseTimersAction : Action
    {
        [PluginProperty("Name", "Name of the timers to pause")]
        public ValueReader Name { get; set; }

        public override void Run()
        {
            Event.Entity.Scene.PauseTimers(Name);
        }
    }
}
