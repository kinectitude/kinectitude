using System.Linq;
using Coding4Fun.Kinect.Wpf;
using Kinectitude.Core.Base;
using Microsoft.Kinect;

namespace Kinectitude.Kinect
{
    public class KinectManager : Manager<KinectComponent>
    {
        private static KinectSensor kinectDriver = null;
        private const int scount = 6;
        private static Skeleton[] allSkeletons = new Skeleton[scount];

        public KinectManager() : base() 
        {
            enable();
        }

        private void enable()
        {
            if (0 != KinectSensor.KinectSensors.Count)
            {
                kinectDriver = KinectSensor.KinectSensors.First();
                kinectDriver.SkeletonStream.Enable(new TransformSmoothParameters()
                {
                    Smoothing = 0.5f,
                    Correction = 0.5f,
                    Prediction = 0.5f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.04f
                });
                kinectDriver.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                kinectDriver.AllFramesReady += sensorReady;
                kinectDriver.Start();
            }
        }

        private Skeleton[] getSkeletons(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (null == frame)
                {
                    return null;
                }
                frame.CopySkeletonDataTo(allSkeletons);
                Skeleton[] skeletons = (from s in allSkeletons
                                         where s.TrackingState != SkeletonTrackingState.NotTracked
                                         select s).ToArray();
                return skeletons;
            }
        }

        private void sensorReady(object sender, AllFramesReadyEventArgs e)
        {
            if (!Running)
            {
                return;
            }
            Skeleton[] skeletons = getSkeletons(e);
            if (null != skeletons && 0 != skeletons.Length)
            {
                using (DepthImageFrame depth = e.OpenDepthImageFrame())
                {
                    if (null == depth)
                    {
                        return;
                    }
                    foreach (KinectComponent kc in Children)
                    {
                        KinectFollowComponent kfc = (KinectFollowComponent)kc;
                        if (KinectFollowComponent.PlayerType.P1 == kfc.followPlayer)
                        {
                            Joint scaledJoint = SkeletalExtensions.ScaleTo(skeletons[0].Joints[kfc.Joint], 800, 600 - 128, 0.4f, 0.4f);
                            kfc.UpdatePosition(scaledJoint.Position.X, scaledJoint.Position.Y);
                        }
                        else if (skeletons.Length > 1)
                        {
                            Joint scaledJoint = SkeletalExtensions.ScaleTo(skeletons[1].Joints[kfc.Joint], 800, 600 - 128, 0.4f, 0.4f);
                            kfc.UpdatePosition(scaledJoint.Position.X, scaledJoint.Position.Y);
                        }
                    }
                }
            }
        }

        public override void OnUpdate(float t)
        {
            if (null == kinectDriver && 0 != KinectSensor.KinectSensors.Count)
            {
                enable();
            }
            foreach (KinectComponent kc in Children)
            {
                kc.OnUpdate(t);
            }
        }
    }
}
