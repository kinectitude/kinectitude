using System;
using Kinectitude.Core;
using Kinectitude.Core.Base;

namespace Kinectitude.Kinect
{
    public abstract class KinectComponent : Component
    {
        protected KinectComponent(Entity entity) : base(entity) { }
        override public Type ManagerType() { return typeof(KinectManager); }
    }
}
