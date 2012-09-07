using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    [Plugin("Create timer {Name}: (Duration: {Duration}, Trigger: {Trigger}, Recurring? {Recurring})", "")]
    internal class CreateTimerAction : Action
    {
        [Plugin("Name", "Name of the timer to create")]
        public IExpressionReader Name { get; set; }

        [Plugin("Duration", "Duration, in seconds, to wait before triggering the trigger")]
        public IDoubleExpressionReader Duration { get; set; }

        [Plugin("Trigger", "Trigger to fire")]
        public IExpressionReader Trigger { get; set; }

        [Plugin("Recurring", "Determines if the timer should start again when it has finished")]
        public IBoolExpressionReader Recurring { get; set; }

        public CreateTimerAction()
        {
            Recurring = new BoolExpressionReader("false", null, null);
        }

        public override void Run()
        {
            Event.Entity.Scene.AddTimer(Name.GetValue(), (float)Duration.GetValue(), Trigger, Recurring.GetValue());
        }
    }
}
