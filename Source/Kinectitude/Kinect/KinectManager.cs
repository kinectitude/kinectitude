using System.Linq;
using Kinectitude.Core.Base;
using Microsoft.Kinect;

namespace Kinectitude.Kinect
{
    public class KinectManager : Manager<KinectComponent>
    {
        private static KinectSensor kinectDriver = null;
        private const int scount = 6;
        private static Skeleton[] allSkeletons = new Skeleton[scount];

        private KinectManager currentManager = null;

        public KinectManager()
        {
            enable();
            currentManager = this;
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
                        float scaleValue = 0.5f;  // Experimentally determined value that we may need to calibrate upon startup based on the lowest hand position we want
                        float x = -1f, y = -1f;
                        if (KinectFollowComponent.PlayerType.P1 == kfc.Player)
                        {
                            x = skeletons[0].Joints[kfc.Joint].Position.X;
                            y = skeletons[0].Joints[kfc.Joint].Position.Y;
                        }
                        else if (skeletons.Length > 1)
                        {
                            x = skeletons[1].Joints[kfc.Joint].Position.X;
                            y = skeletons[1].Joints[kfc.Joint].Position.Y;
                        }

                        if (x != -1 && y != -1)
                        {
                            y = (float)((1 - (y + 1) / 2) / scaleValue);
                            if (y > 1) y = 1;
                            if (y < 0) y = 0;
                        }

                        kfc.UpdatePosition(x * 800, y * 600);
                    }
                }
            }
        }

        public override void OnUpdate(float t)
        {
            if (null == kinectDriver && 0 != KinectSensor.KinectSensors.Count)
            {
                enable();
                kinectDriver.AllFramesReady += sensorReady;
            }
            foreach (KinectComponent kc in Children)
            {
                kc.OnUpdate(t);
            }
        }

        protected override void OnStart()
        {
            if (null != kinectDriver)
            {
                kinectDriver.AllFramesReady += sensorReady;
            }
        }

        protected override void OnStop()
        {
            if (null != kinectDriver)
            {
                kinectDriver.AllFramesReady -= sensorReady;
            }
        }
    }
}
