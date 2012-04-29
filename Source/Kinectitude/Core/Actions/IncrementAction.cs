using System.Xml;
using System;
using Kinectitude.Attributes;

namespace Kinectitude.Core
{
    [Plugin("Increment an attribute", "")]
    public sealed class IncrementAction : Action
    {
        [Plugin("Key", "")]
        public SpecificWriter Target { get; set; }

        [Plugin("Amount", "")]
        public double IncrementBy { get; set; }

        public IncrementAction()
        {
            IncrementBy = 1;
        }

        public override void Run()
        {
            Target.SetValue((double.Parse(Target.GetValue()) + IncrementBy).ToString());
        }
    }
}
