using FarseerPhysics.Dynamics;
using Kinectitude.Core.Base;
using Microsoft.Xna.Framework;

namespace Kinectitude.Physics
{
    public class PhysicsManager : Manager<PhysicsComponent>
    {

        public World PhysicsWorld { get; private set; }

        public PhysicsManager(Game game) : base(game)
        {
            PhysicsWorld = new World(Vector2.Zero);
            PhysicsWorld.ClearForces();
        }

        public override void OnUpdate(float t)
        {
            PhysicsWorld.Step(t);
            foreach (PhysicsComponent pc in children)
            {
                pc.OnUpdate(t);
            }
        }

        protected override void OnAdd(PhysicsComponent item)
        {
            item.Initialize(this);
        }
    }
}
