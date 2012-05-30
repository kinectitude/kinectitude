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
        public double IncrementBy { get; set; }

        public IncrementAction()
        {
            IncrementBy = 1;
        }

        public override void Run()
        {
            Target.Value = (double.Parse(Target.Value) + IncrementBy).ToString();
        }
    }
}
