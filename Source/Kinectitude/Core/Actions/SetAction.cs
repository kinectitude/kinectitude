using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    internal sealed class SetAction : Action
    {
        private readonly ValueReader Value;
        private readonly ValueWriter Target;

        public SetAction(ValueWriter target, ValueReader value, Event evt) 
        {
            Target = target;
            Value = value;
            Event = evt;
        }

        public override void Run()
        {
            Target.SetValue(Value);
        }
    }
}
