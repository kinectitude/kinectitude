using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Events
{
    [Plugin("Expression {Value} equals {Target}", "Triggers when two expressions are equal")]
    internal sealed class AttributeEqualsEvent : Event
    {
        [Plugin("Left", "")]
        public IExpressionReader Value { get; set; }

        [Plugin("Right", "")]
        public IExpressionReader Target { get; set; }

        public AttributeEqualsEvent() { }

        public override void OnInitialize()
        {

            Value.notifyOfChange(Trigger);
            Target.notifyOfChange(Trigger);
        }

        public void Trigger(string newValue)
        {
            if (Value.GetValue() == Target.GetValue())
            {
                DoActions();
            }
        }
    }
}