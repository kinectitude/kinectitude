using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Events
{
    // Trigger can just be an event that does nothing special on its own, but can be treated special

    [Plugin("Trigger occurs", "")]
    internal class TriggerOccursEvent : Event
    {

        public TriggerOccursEvent() { }

        [Plugin("Trigger", "")]
        public string Trigger { get; set; }

        public override void OnInitialize()
        {
            Scene scene = Entity.Scene;
            scene.RegisterTrigger(Trigger, this);
        }
    }
}
