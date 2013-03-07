using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Physics
{
    [Plugin("when this entity collides with {CollidesWith}", "Entity collides")]
    public class CollisionEvent : Event
    {
        [PluginProperty("Collides With","")]
        public TypeMatcher CollidesWith { get; set; }

        public CollisionEvent() { }

        public override void OnInitialize()
        {
            PhysicsComponent pc = GetComponent<PhysicsComponent>();
            pc.AddCollisionEvent(this);
        }
    }
}
