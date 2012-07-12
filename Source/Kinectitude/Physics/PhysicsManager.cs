using FarseerPhysics.Dynamics;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Microsoft.Xna.Framework;

namespace Kinectitude.Physics
{
    public class PhysicsManager : Manager<PhysicsComponent>
    {

        private float yGravity;
        [Plugin("Y gravity", "How fast things are pulled down")]
        public float YGravity 
        {
            get { return yGravity; }
            set
            {
                if (yGravity != value)
                {
                    yGravity = value;
                    Change("YGravity");
                }
                PhysicsWorld.Gravity = new Vector2(xGravity, yGravity);
            }
        }

        private float xGravity;
        [Plugin("X gravity", "How fast things are pulled to the left")]
        public float XGravity 
        {
            get { return xGravity; }
            set
            {
                if (xGravity != value)
                {
                    xGravity = value;
                    Change("XGravity");
                }
                PhysicsWorld.Gravity = new Vector2(xGravity, yGravity);
            }
        }

        public World PhysicsWorld { get; private set; }

        public PhysicsManager()
        {
            PhysicsWorld = new World(Vector2.Zero);
            PhysicsWorld.ClearForces();
        }

        public override void OnUpdate(float t)
        {
			foreach (PhysicsComponent pc in Children)
            {
                pc.SetPosition();
            }
		
            PhysicsWorld.Step(t);
			
            foreach (PhysicsComponent pc in Children)
            {
                pc.OnUpdate(t);
            }
        }
    }
}
