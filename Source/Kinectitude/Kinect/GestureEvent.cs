using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Microsoft.Kinect;
using Kinect.Toolbox;
using System.IO;

namespace Kinectitude.Kinect
{
    [Plugin("Player {Player} performs gesture {GestureName} with joint {Joint}", "")]
    public class GestureEvent : Event
    {
        private KinectManager manager;
        private const string folder = "Kinectitude.Kinect.Data.";
        private const string extention = ".save";

        public TemplatedGestureDetector GestureDetector { get; private set; }

        [Plugin("Gesture", "Name of the gesture to detect")]
        public string GestureName { get; set; }

        [Plugin("Joint", "Joint to detect gesture on")]
        public JointType Joint { get; set; }

        [Plugin("Player", "player to detect gesture from")]
        public int Player { get; set; }

        public override void OnInitialize()
        {
            manager = GetManager<KinectManager>();
            manager.AddGestureEvent(this);
            Stream gestureStream =
                typeof(GestureEvent).Assembly.GetManifestResourceStream(folder + GestureName + extention);

            //TODO null == gesture is if the file is not there.

            GestureDetector = new TemplatedGestureDetector(GestureName, gestureStream);
            GestureDetector.MinimalPeriodBetweenGestures = 0;
            GestureDetector.OnGestureDetected += detectedGesture;
        }

        private void detectedGesture(string gestureName) { DoActions(); }

        public override void Destroy()
        {
            manager.RemoevGestureEvent(this);
            GestureDetector.OnGestureDetected -= detectedGesture;
        }
    }
}
