using Kinectitude.Core.Base;

namespace Kinectitude.Core.Interfaces
{
    public interface IPhysicsComponent : IUpdateable
    {
        float XVelocity { get; set; }
        float YVelocity { get; set; }
        float AngularVelocity { get; set; }
    }
}
