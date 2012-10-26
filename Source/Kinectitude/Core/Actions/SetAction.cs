using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    [Plugin("Set value {Target} to {Value}", "")]
    public sealed class SetAction : Action
    {
        [Plugin("Value", "Value to set with")]
        public ValueReader Value { get; set; }

        [Plugin("Key", "Attribute Or property to change")]
        public ValueWriter Target { get; set; }

        public SetAction() { }

        public override void Run()
        {
            Target.SetValue(Value);
        }
    }
}
