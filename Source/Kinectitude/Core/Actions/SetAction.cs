using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    [Plugin("Set value {Target} to {Value}", "")]
    public sealed class SetAction : Action
    {
        [Plugin("Value", "Value to set with")]
        public IExpressionReader Value { get; set; }

        [Plugin("Key", "Attribute or property to change")]
        public IValueWriter Target { get; set; }

        public SetAction() { }

        public override void Run()
        {
            Target.Value = Value.GetValue();
        }
    }
}
