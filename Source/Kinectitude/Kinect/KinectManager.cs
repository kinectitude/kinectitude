using Kinectitude.Core.Base;
using Microsoft.Kinect;

namespace Kinectitude.Kinect
{
    public class KinectManager : Manager<KinectComponent>
    {
        private static KinectService service;
        private Skeleton[] latestSkeletons = null;

        private void update(Skeleton[] skeletons)
        {
            latestSkeletons = skeletons;
        }

        public override void OnUpdate(float t)
        {
            if (null == latestSkeletons)
            {
                return;
            }
            
            foreach (KinectFollowComponent kfc in Children)
            {
                float scaleValue = 0.5f;  // Experimentally determined value that we may need to calibrate upon startup based on the lowest hand position we want
                float x = -1f, y = -1f;
                if (1 == kfc.Player)
                {
                    x = latestSkeletons[0].Joints[kfc.Joint].Position.X;
                    y = latestSkeletons[0].Joints[kfc.Joint].Position.Y;
                }
                else if (latestSkeletons.Length > 1 && 2 == kfc.Player)
                {
                    x = latestSkeletons[1].Joints[kfc.Joint].Position.X;
                    y = latestSkeletons[1].Joints[kfc.Joint].Position.Y;
                }

                if (x != -1 && y != -1)
                {
                    y = (float)((1 - (y + 1) / 2) / scaleValue);
                    if (y > 1) y = 1;
                    if (y < 0) y = 0;
                }

                kfc.UpdatePosition(x * 800, y * 600);
                kfc.OnUpdate(t);
            }

            latestSkeletons = null;
        }

        protected override void OnStart()
        {
            if (null == service)
            {
                service = GetService<KinectService>();
            }
            service.Callback = update;
        }

        protected override void OnStop()
        {
            service.Callback = null;
        }
    }
}
