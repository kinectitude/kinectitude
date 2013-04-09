//-----------------------------------------------------------------------
// <copyright file="KinectManager.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Base;
using Microsoft.Kinect;
using System.Collections.Generic;
using System;
using Kinectitude.Core.Attributes;
using Kinectitude.Render;

namespace Kinectitude.Kinect
{
    [Plugin("Kinect Manager", "")]
    public class KinectManager : Manager<KinectFollowComponent>
    {
        private const int NumPlayers = 2;
        private static readonly int NumJoints = Enum.GetValues(typeof(JointType)).Length;

        private readonly List<GestureEvent>[][] events = new List<GestureEvent>[NumPlayers][];
        private KinectService kinectService;
        private Tuple<int, int> windowSize;
        private Skeleton[] latestSkeletons;

        public CoordinateMapper CoordinateMapper
        {
            get { return kinectService.KinectSensor.CoordinateMapper; }
        }

        public Tuple<int, int> WindowSize
        {
            get { return windowSize; }
        }

        public KinectManager()
        {
            for (int i = 0; i < NumPlayers; i++)
            {
                events[i] = new List<GestureEvent>[NumJoints];
                for (int j = 0; j < NumJoints; j++) events[i][j] = new List<GestureEvent>();
            }
        }

        public override void OnUpdate(float frameDelta)
        {
            if (null == latestSkeletons) return;
            
            foreach (KinectFollowComponent kfc in Children)
            {
                if (latestSkeletons.Length >= kfc.Player)
                {
                    Joint joint = latestSkeletons[kfc.Player - 1].Joints[kfc.Joint];
                    //var point = kinectService.KinectSensor.CoordinateMapper.MapSkeletonPointToColorPoint(joint.Position, ColorImageFormat.RgbResolution640x480Fps30);
                    //float scaleX = windowSize.Item1 / 640.0f;
                    //float scaleY = windowSize.Item2 / 480.0f;

                    kfc.UpdateJoint(joint);
                    //kfc.UpdatePosition(point.X * scaleX, point.Y * scaleY);
                    kfc.OnUpdate(frameDelta);
                }
            }

            for (int i = 0; i < NumPlayers && latestSkeletons.Length > i; i++)
            {
                for (int j = 0; j < NumJoints; j++)
                {
                    foreach (GestureEvent gestureEvent in events[i][j])
                    {
                        gestureEvent.GestureDetector.Add
                            (latestSkeletons[i].Joints[gestureEvent.Joint].Position, kinectService.KinectSensor);
                    }
                }
            }

            latestSkeletons = null;
        }

        private void OnSkeletonsReady(Skeleton[] skeletons)
        {
            latestSkeletons = skeletons;
        }

        protected override void OnStart()
        {
            kinectService = GetService<KinectService>();
            kinectService.SkeletonsReady = OnSkeletonsReady;
            
            RenderService renderService = GetService<RenderService>();
            windowSize = Tuple.Create((int)renderService.Width, (int)renderService.Height);
        }

        protected override void OnStop()
        {
            kinectService.SkeletonsReady -= OnSkeletonsReady;
        }

        public void AddGestureEvent(GestureEvent gestureEvent)
        { 
            events[gestureEvent.Player - 1][(int)gestureEvent.Joint].Add(gestureEvent);
        }

        public void RemoveGestureEvent(GestureEvent gestureEvent)
        {
            events[gestureEvent.Player - 1][(int)gestureEvent.Joint].Remove(gestureEvent); 
        }
    }
}
