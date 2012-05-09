using Kinectitude.Attributes;
using Action = Kinectitude.Core.Base.Action;

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
            Event.Scene.FireTrigger(Name);
        }
    }
}
