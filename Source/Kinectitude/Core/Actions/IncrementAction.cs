using Kinectitude.Core.Attributes;
using Kinectitude.Core.Data;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Actions
{
    [Plugin("Increment value {Target} by {IncrementBy}", "")]
    internal sealed class IncrementAction : Action
    {
        [Plugin("Key", "")]
        public ValueWriter Target { get; set; }

        [Plugin("Amount", "")]
        public ValueReader IncrementBy { get; set; }

        public IncrementAction()
        {
            IncrementBy = new ConstantReader(1);
        }

        public override void Run()
        {
            if (null != Target) Target.SetValue(Target.GetDoubleValue() + IncrementBy.GetDoubleValue());
        }
    }
}
