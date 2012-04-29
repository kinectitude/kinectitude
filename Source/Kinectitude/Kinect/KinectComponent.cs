using System;
using Kinectitude.Core;

namespace Kinectitude.Kinect
{
    public abstract class KinectComponent : Component
    {
        protected KinectComponent(Entity entity) : base(entity) { }
        override public Type ManagerType() { return typeof(KinectManager); }
    }
}
