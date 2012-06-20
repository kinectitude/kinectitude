using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;
using System;

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

        float MaximumVelocity { get; set; }

        float MinimumVelocity { get; set; }
    }
}
