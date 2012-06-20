using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Events
{
    // Trigger can just be an event that does nothing special on its own, but can be treated special

    [Plugin("Trigger occurs", "")]
    internal class TriggerOccursEvent : Event
    {

        public TriggerOccursEvent() { }

        [Plugin("Trigger", "")]
        public IExpressionReader Trigger { get; set; }

        public override void OnInitialize()
        {
            Scene scene = Entity.Scene;
            scene.RegisterTrigger(Trigger.GetValue(), this);
        }
    }
}
