using Kinectitude.Attributes;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Events
{
    [Plugin("Attribute equals a value", "")]
    internal class AttributeEqualsEvent : AttributeChangesEvent
    {
        [Plugin("Value", "")]
        public string Value { get; set; }

        public AttributeEqualsEvent() { }

        public override bool Trigger(DataContainer dataContainer)
        {
            return dataContainer[Key].Equals(Value);
        }
    }
}