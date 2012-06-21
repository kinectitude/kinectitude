using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Physics
{
    [Plugin("Fire an event when a collision between bodies occurs", "")]
    public class CollisionEvent : Event
    {
        [Plugin("Collides with","")]
        public ITypeMatcher CollidesWith { get; set; }

        public CollisionEvent() { }

        public override void OnInitialize()
        {
            PhysicsComponent pc = GetComponent<PhysicsComponent>();
            pc.AddCollisionEvent(this);
        }
    }
}
