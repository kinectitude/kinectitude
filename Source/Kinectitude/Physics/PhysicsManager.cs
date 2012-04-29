using Kinectitude.Core;

namespace Kinectitude.Physics
{
    public class PhysicsManager : Manager<IPhysics>
    {
        public PhysicsManager(Game g) : base(g) { }

        public override void OnUpdate(double t)
        {
            for (int i = 0; i < children.Count; i++)
            {
                for (int j = i + 1; j < children.Count; j++)
                {
                    IPhysics pc = children[i];
                    IPhysics pc2 = children[j];

                    if (pc.HitTest(pc2, t))
                    {
                        if ((pc2.Dx - pc.Dx > 0 && pc2.X < pc.X) || (pc2.Dx - pc.Dx < 0 && pc2.X > pc.X)) // If the objects are heading towards each other
                        {
                            pc.Dx = -pc.Dx;
                            pc2.Dx = -pc2.Dx;
                        }

                        if (pc.Above(pc2) || pc2.Above(pc))
                        {
                            pc.Dy = -pc.Dy;
                            pc2.Dy = -pc2.Dy;
                        }
                    }
                }
            }

            foreach (PhysicsComponent pc in children)
            {
                pc.OnUpdate(t);
            }
        }
    }
}
