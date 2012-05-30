﻿using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    [Plugin("Set an attribute", "")]
    public sealed class SetAttributeAction : Action
    {
        [Plugin("Value", "Value to set the attribute to")]
        public IExpressionReader Value { get; set; }

        [Plugin("Key", "Attribute to change")]
        public IValueWriter Target { get; set; }

        public SetAttributeAction() { }

        public override void Run()
        {
            Target.Value = Value.GetValue();
        }
    }
}
