//-----------------------------------------------------------------------
// <copyright file="GestureEvent.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

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
    [Plugin("when player {Player} performs gesture {GestureName} with joint {Joint}", "Player makes a gesture")]
    public class GestureEvent : Event
    {
        private KinectManager manager;
        private const string folder = "Kinectitude.Kinect.DataOld.";
        private const string extention = ".save";

        public TemplatedGestureDetector GestureDetector { get; private set; }

        [PluginProperty("Gesture", "Name of the gesture to detect")]
        public string GestureName { get; set; }

        [PluginProperty("Joint", "Joint to detect gesture on")]
        public JointType Joint { get; set; }

        [PluginProperty("Player", "player to detect gesture from", 1)]
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
            manager.RemoveGestureEvent(this);
            GestureDetector.OnGestureDetected -= detectedGesture;
        }
    }
}
