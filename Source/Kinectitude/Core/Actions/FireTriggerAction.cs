using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    [Plugin("Fire trigger {Name}", "")]
    internal sealed class FireTriggerAction : Action
    {
        [PluginProperty("Trigger", "")]
        public ValueReader Name { get; set; }

        public FireTriggerAction() { }

        public override void Run()
        {
            Event.Entity.Scene.FireTrigger(Name);
        }
    }
}
