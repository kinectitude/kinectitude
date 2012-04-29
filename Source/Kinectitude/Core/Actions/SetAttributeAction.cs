using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Kinectitude.Attributes;

namespace Kinectitude.Core
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
