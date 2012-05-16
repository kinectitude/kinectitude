using Kinectitude.Core;
using Kinectitude.Core.Base;
using Kinectitude.Core.Interfaces;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;

namespace Kinectitude.Physics
{
    public class PhysicsManager : Manager<PhysicsComponent>
    {

        public World PhysicsWorld { get; private set; }

        public PhysicsManager(Game g) : base(g)
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
