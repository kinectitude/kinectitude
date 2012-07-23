﻿using System;
using System.Linq;
using Kinectitude.Core.Base;
using Microsoft.Kinect;

namespace Kinectitude.Kinect
{
    public class KinectService : Service
    {

        public static KinectSensor KinectDriver { get; private set; }
        private const int scount = 6;
        private static Skeleton[] allSkeletons = new Skeleton[scount];

        public Action<Skeleton[]> Callback { get; set; }

        private void enable()
        {
            if (0 != KinectSensor.KinectSensors.Count)
            {
                KinectDriver = KinectSensor.KinectSensors.First();
                KinectDriver.SkeletonStream.Enable(new TransformSmoothParameters()
                {
                    Smoothing = 0.5f,
                    Correction = 0.5f,
                    Prediction = 0.5f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.04f
                });
                KinectDriver.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                KinectDriver.Start();
                if (null != KinectDriver)
                {
                    KinectDriver.AllFramesReady += sensorReady;
                }
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
                    if (null != Callback)
                    {
                        Callback(skeletons);
                    }
                }
            }
        }

        public override void OnStart()
        {
            enable();
        }

        public override void OnStop()
        {
            KinectDriver.Dispose();
        }

        public override bool AutoStart()
        {
            return true;
        }
    }
}
