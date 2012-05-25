using Kinectitude.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Events
{
    [Plugin("Attribute equals a value", "Triggers when the target becomes equal to the value")]
    internal sealed class AttributeEqualsEvent : Event
    {
        [Plugin("Value", "")]
        public IExpressionReader Value { get; set; }

        [Plugin("Target", "")]
        public IExpressionReader Target { get; set; }

        public AttributeEqualsEvent() { }

        public override void OnInitialize()
        {

            Value.notifyOfChange(Trigger);
            Target.notifyOfChange(Trigger);
        }

        public void Trigger(string oldValue, string newValue)
        {
            if (Value.GetValue() == Target.GetValue())
            {
                DoActions();
            }
        }
    }
}