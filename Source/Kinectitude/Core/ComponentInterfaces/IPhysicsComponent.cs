using Kinectitude.Core.Base;

namespace Kinectitude.Core.ComponentInterfaces
{
    public interface IPhysics : IUpdateable
    {
        float XVelocity { get; set; }

        float YVelocity { get; set; }

        float AngularVelocity { get; set; }

        float Restitution { get; set; }

        float Mass { get; set; }

        float Friction { get; set; }

        float LinearDamping { get; set; }

        float MaximumSpeed { get; set; }

        float MinimumSpeed { get; set; }

        bool MovesWhenHit { get; set; }
    }
}
