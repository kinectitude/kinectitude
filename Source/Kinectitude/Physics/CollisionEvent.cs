using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Physics
{
    [Plugin("Entity collides with {CollidesWith}", "")]
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
