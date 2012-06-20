using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    [Plugin("Creates a timer", "")]
    internal class CreateTimerAction : Action
    {
        [Plugin("Name of the timer to create", "")]
        public IExpressionReader Name { get; set; }

        [Plugin("Duration, in seconds, to wait before triggering the trigger", "")]
        public IDoubleExpressionReader Duration { get; set; }

        [Plugin("Trigger to fire", "")]
        public IExpressionReader Trigger { get; set; }

        [Plugin("Determins if the timer should start again when it has finished", "")]
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
