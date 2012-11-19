using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Events
{
    [Plugin("Expression {Value} equals {Target}", "Triggers when two expressions are equal")]
    internal sealed class AttributeEqualsEvent : Event
    {
        [Plugin("Left", "")]
        public ValueReader Value { get; set; }

        [Plugin("Right", "")]
        public ValueReader Target { get; set; }

        public AttributeEqualsEvent() { }

        public override void OnInitialize()
        {

            Value.NotifyOfChange(Trigger);
            Target.NotifyOfChange(Trigger);
        }

        public void Trigger(ValueReader newValue)
        {
            if (Value.HasSameVal(Target)) DoActions();
        }
    }
}