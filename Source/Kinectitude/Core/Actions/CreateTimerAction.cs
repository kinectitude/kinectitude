using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    [Plugin("Create timer {Name}: (Duration: {Duration}, Trigger: {Trigger}, Recurring? {Recurring})", "")]
    internal sealed class CreateTimerAction : Action
    {
        [PluginProperty("Name", "Name of the timer to create")]
        public ValueReader Name { get; set; }

        [PluginProperty("Duration", "Duration, in seconds, to wait before triggering the trigger")]
        public ValueReader Duration { get; set; }

        [PluginProperty("Trigger", "Trigger to fire")]
        public ValueReader Trigger { get; set; }

        [PluginProperty("Recurring", "Determines if the timer should start again when it has finished", true)]
        public ValueReader Recurring { get; set; }

        public CreateTimerAction()
        {
            Recurring = new ConstantReader(false);
        }

        public override void Run()
        {
            Event.Entity.Scene.AddTimer(Name, Duration, Trigger, Recurring);
        }
    }
}
