using Kinectitude.Attributes;
using Action = Kinectitude.Core.Base.Action;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    [Plugin("Increment an attribute", "")]
    public sealed class IncrementAction : Action
    {
        [Plugin("Key", "")]
        public SpecificWriter Target { get; set; }

        [Plugin("Amount", "")]
        public double IncrementBy { get; set; }

        public IncrementAction()
        {
            IncrementBy = 1;
        }

        public override void Run()
        {
            Target.SetValue((double.Parse(Target.GetValue()) + IncrementBy).ToString());
        }
    }
}
