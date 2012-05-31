using Kinectitude.Core.Attributes;
using Kinectitude.Core.Data;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Actions
{
    [Plugin("Increment an attribute", "")]
    internal sealed class IncrementAction : Action
    {
        [Plugin("Key", "")]
        public IValueWriter Target { get; set; }

        [Plugin("Amount", "")]
        public IExpressionReader IncrementBy { get; set; }

        public IncrementAction() { }

        public override void Run()
        {
            if (null == IncrementBy)
            {
                IncrementBy = ExpressionReader.CreateExpressionReader("1", Event, Event.Entity);
            }
            Target.Value = (double.Parse(Target.Value) + double.Parse(IncrementBy.GetValue())).ToString();
        }
    }
}
