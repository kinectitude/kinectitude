using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Attributes;

namespace Kinectitude.Core
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