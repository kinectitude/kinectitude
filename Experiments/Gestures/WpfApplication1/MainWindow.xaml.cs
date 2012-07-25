using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Kinect.Toolbox;
using Kinect.Toolbox.Record;
using System.IO;
using Microsoft.Kinect;
using Microsoft.Win32;
using Kinect.Toolbox.Voice;

namespace WpfApplication1
{
    public partial class MainWindow
    {
        KinectSensor kinectSensor;
        SwipeGestureDetector swipeGestureRecognizer;
        TemplatedGestureDetector circleGestureRecognizer;
<<<<<<< HEAD
        TemplatedGestureDetector handUpGestureRecognizer;
        readonly BarycenterHelper barycenterHelper = new BarycenterHelper();

        string circleKBPath;
        string handUpPath;
=======
        readonly BarycenterHelper barycenterHelper = new BarycenterHelper();

        string circleKBPath;
>>>>>>> 11c5ca39c6e057b16bc2e5c5721c1ab574d74abd

        private Skeleton[] skeletons;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
<<<<<<< HEAD
            circleKBPath = Path.Combine(Environment.CurrentDirectory, @"data\circleKB.save");
            handUpPath = Path.Combine(Environment.CurrentDirectory, @"data\moveHandUp.save");
=======
            circleKBPath = Path.Combine(Environment.CurrentDirectory, @"../../Data\circleKB.save");
>>>>>>> 11c5ca39c6e057b16bc2e5c5721c1ab574d74abd

            try
            {
                //loop through all the Kinects attached to this PC, and start the first that is connected without an error.
                foreach (KinectSensor kinect in KinectSensor.KinectSensors)
                {
                    if (kinect.Status == KinectStatus.Connected)
                    {
                        kinectSensor = kinect;
                        break;
                    }
                }

                if (KinectSensor.KinectSensors.Count == 0)
                    MessageBox.Show("No Kinect found");
                else
                    Initialize();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Initialize()
        {
            if (kinectSensor == null)
                return;

            kinectSensor.SkeletonStream.Enable(new TransformSmoothParameters
            {
                Smoothing = 0.5f,
                Correction = 0.5f,
                Prediction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            });
            kinectSensor.SkeletonFrameReady += kinectRuntime_SkeletonFrameReady;

            swipeGestureRecognizer = new SwipeGestureDetector();
            swipeGestureRecognizer.OnGestureDetected += OnGestureDetected;

            kinectSensor.Start();

            LoadCircleGestureDetector();
<<<<<<< HEAD
            LoadHandUpGestureDetector();
=======
>>>>>>> 11c5ca39c6e057b16bc2e5c5721c1ab574d74abd
        }

        void kinectRuntime_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame == null)
                    return;

                Tools.GetSkeletons(frame, ref skeletons);

                if (skeletons.All(s => s.TrackingState == SkeletonTrackingState.NotTracked))
                    return;

                ProcessFrame(frame);
            }
        }

        void ProcessFrame(ReplaySkeletonFrame frame)
        {
            foreach (var skeleton in frame.Skeletons)
            {
                if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                    continue;

                barycenterHelper.Add(skeleton.Position.ToVector3(), skeleton.TrackingId);
                if (!barycenterHelper.IsStable(skeleton.TrackingId))
                    return;

                foreach (Joint joint in skeleton.Joints)
                {
                    if (joint.TrackingState != JointTrackingState.Tracked)
                        continue;

                    if (joint.JointType == JointType.HandRight)
                    {
                        swipeGestureRecognizer.Add(joint.Position, kinectSensor);
                        circleGestureRecognizer.Add(joint.Position, kinectSensor);
<<<<<<< HEAD
                        handUpGestureRecognizer.Add(joint.Position, kinectSensor);
=======
>>>>>>> 11c5ca39c6e057b16bc2e5c5721c1ab574d74abd
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Clean();
        }

        private void Clean()
        {
            if (swipeGestureRecognizer != null)
            {
                swipeGestureRecognizer.OnGestureDetected -= OnGestureDetected;
            }

            CloseGestureDetector();

            if (kinectSensor != null)
            {
                kinectSensor.SkeletonFrameReady -= kinectRuntime_SkeletonFrameReady;
                kinectSensor.Stop();
                kinectSensor = null;
            }
        }
    }
}
