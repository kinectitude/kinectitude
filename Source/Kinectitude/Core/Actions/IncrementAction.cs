using Kinectitude.Core.Attributes;
using Kinectitude.Core.Data;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Actions
{
    [Plugin("Increment value {Target} by {IncrementBy}", "")]
    internal sealed class IncrementAction : Action
    {
        [Plugin("Key", "")]
        public IValueWriter Target { get; set; }

        [Plugin("Amount", "")]
        public IDoubleExpressionReader IncrementBy { get; set; }

        public IncrementAction() { }

        public override void Run()
        {
            if (null == IncrementBy)
            {
                IncrementBy = new DoubleExpressionReader("1", Event, Event.Entity);
            }
            if (null != Target.Value) Target.Value = (double.Parse(Target.Value) + IncrementBy.GetValue()).ToString();
            else Target.Value = "1";
        }
    }
}
