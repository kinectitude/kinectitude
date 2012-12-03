using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Events
{
    // Trigger can just be an event that does nothing special on its own, but can be treated special

    [Plugin("Trigger {Trigger} occurs", "")]
    internal sealed class TriggerOccursEvent : Event
    {

        public TriggerOccursEvent() { }

        [PluginProperty("Trigger", "")]
        public string Trigger { get; set; }

        public override void OnInitialize()
        {
            Scene scene = Entity.Scene;
            scene.RegisterTrigger(Trigger, this);
        }
    }
}
