using System;
using System.Linq;
using Kinectitude.Core.Base;
using Microsoft.Kinect;
using Microsoft.Speech.Recognition;
using System.IO;

namespace Kinectitude.Kinect
{
    public class KinectService : Service
    {
        public KinectSensor KinectSensor { get; private set; }

        public Action<Skeleton[]> SkeletonsReady { get; set; }

        //public Action<string> SpeechCallback { get; set; }

        //private readonly SpeechRecognitionEngine speechRecognitionEngine;

        private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = null;

            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (null != frame)
                {
                    skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                }

                if (null != skeletons && null != SkeletonsReady)
                {
                    SkeletonsReady(skeletons.Where(x => x.TrackingState != SkeletonTrackingState.NotTracked).ToArray());
                }
            }
        }

        public override void OnStart()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    KinectSensor = potentialSensor;
                    break;
                }
            }

            if (null != KinectSensor)
            {
                KinectSensor.SkeletonStream.Enable(new TransformSmoothParameters()
                {
                    Smoothing = 0.5f,
                    Correction = 0.5f,
                    Prediction = 0.5f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.04f
                });
                KinectSensor.SkeletonFrameReady += OnSkeletonFrameReady;

                try
                {
                    KinectSensor.Start();
                }
                catch (IOException)
                {
                    KinectSensor = null;
                }
            }
        }

        public override void OnStop()
        {
            if (KinectSensor != null)
            {
                KinectSensor.SkeletonFrameReady -= OnSkeletonFrameReady;
                KinectSensor.Stop();
            }
        }

        public override bool AutoStart()
        {
            return true;
        }
    }
}
