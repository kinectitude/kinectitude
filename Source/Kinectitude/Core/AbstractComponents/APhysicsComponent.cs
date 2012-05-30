using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;
using System;

namespace Kinectitude.Core.ComponentInterfaces
{
    public abstract class APhysicsComponent : Component, IUpdateable
    {
        [Plugin("Horizontal Velocity", "")]
        public abstract float XVelocity { get; set; }
        [Plugin("Vertical Velocity", "")]
        public abstract float YVelocity { get; set; }
        [Plugin("Angular Velocity", "")]
        public abstract float AngularVelocity { get; set; }

        public override Type ImplementationType
        {
            get { return typeof(APhysicsComponent); }
        }
    }
}
