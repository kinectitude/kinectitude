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
        private static KinectService service;
        private Skeleton[] latestSkeletons = null;

        private const int numPlayers = 2;
        private static readonly int numJoints = Enum.GetValues(typeof(JointType)).Length;
        private readonly List<GestureEvent>[][] events = new List<GestureEvent>[numPlayers][];

        private Tuple<int, int> windowSize;

        private void update(Skeleton[] skeletons)
        {
            latestSkeletons = skeletons;
        }

        public KinectManager()
        {
            for (int i = 0; i < numPlayers; i++)
            {
                events[i] = new List<GestureEvent>[numJoints];
                for (int j = 0; j < numJoints; j++) events[i][j] = new List<GestureEvent>();
            }
        }

        public override void OnUpdate(float frameDelta)
        {
            if (null == latestSkeletons) return;
            
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

                //Tuple<int, int> windowSize = renderService.//GetWindowSize();

                kfc.UpdatePosition(x * windowSize.Item1, y * windowSize.Item2);
                kfc.OnUpdate(frameDelta);
            }

            for (int i = 0; i < numPlayers && latestSkeletons.Length > i; i++)
            {
                for (int j = 0; j < numJoints; j++)
                {
                    foreach (GestureEvent gestureEvent in events[i][j])
                    {
                        gestureEvent.GestureDetector.Add
                            (latestSkeletons[i].Joints[gestureEvent.Joint].Position, KinectService.KinectDriver);
                    }
                }
            }

            latestSkeletons = null;
        }

        private void said(string what)
        {

        }

        protected override void OnStart()
        {
            if (null == service)
            {
                service = GetService<KinectService>();
            }
            service.Callback = update;
            service.SpeechCallback = said;
            
            RenderService renderService = GetService<RenderService>();
            windowSize = Tuple.Create((int)renderService.Width, (int)renderService.Height);
        }

        protected override void OnStop()
        {
            service.Callback = null;
            service.SpeechCallback = null;
        }

        public void AddGestureEvent(GestureEvent gestureEvent)
        { 
            events[gestureEvent.Player - 1][(int)gestureEvent.Joint].Add(gestureEvent);
        }

        public void RemoevGestureEvent(GestureEvent gestureEvent)
        {
            events[gestureEvent.Player - 1][(int)gestureEvent.Joint].Remove(gestureEvent); 
        }

    }
}
