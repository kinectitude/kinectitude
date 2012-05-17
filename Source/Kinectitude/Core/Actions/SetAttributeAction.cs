using Kinectitude.Attributes;
using Kinectitude.Core.Data;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Actions
{
    [Plugin("Set an attribute", "")]
    public sealed class SetAttributeAction : Action
    {
        [Plugin("Value", "")]
        public SpecificReadable Value { get; set; }

        [Plugin("Key", "")]
        public SpecificWriter Target { get; set; }

        public SetAttributeAction() { }

        public override void Run()
        {
            Target.SetValue(Value.GetValue());
        }
    }
}
