using System;
using Kinectitude.Core;
using Kinectitude.Core.Base;

namespace Kinectitude.Kinect
{
    public abstract class KinectComponent : Component
    {
        protected KinectComponent() : base() { }
        override public Type ManagerType() { return typeof(KinectManager); }
    }
}
