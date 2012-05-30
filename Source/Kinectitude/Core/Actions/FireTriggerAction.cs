using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Actions
{
    [Plugin("Fire a trigger", "")]
    internal sealed class FireTriggerAction : Action
    {
        [Plugin("Trigger", "")]
        public string Name { get; set; }

        public FireTriggerAction() { }

        public override void Run()
        {
            Event.Entity.Scene.FireTrigger(Name);
        }
    }
}
