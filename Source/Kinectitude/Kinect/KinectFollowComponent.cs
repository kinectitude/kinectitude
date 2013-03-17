using Kinectitude.Core.Attributes;
using Kinectitude.Core.ComponentInterfaces;
using Kinectitude.Core.Components;
using Microsoft.Kinect;
using Kinectitude.Core.AbstractComponents;

namespace Kinectitude.Kinect
{
    [Plugin("Kinect Motion Component", "")]
    public class KinectFollowComponent : BasicFollowComponent
    {
        private KinectManager manager;

        private JointType joint;
        [PluginProperty("Joint", "", JointType.HandRight)]
        public JointType Joint
        {
            get { return joint; }
            set
            {
                if (joint != value)
                {
                    joint = value;
                    Change("Joint");
                }
            }
        }

        private int player;
        [PluginProperty("Player", "", 1)]
        public int Player
        {
            get { return player; }
            set
            {
                if (player != value)
                {
                    player = value;
                    Change("Player");
                }
            }
        }

        private int scaleWidth;
        [PluginProperty("Scale Width", "", 0)]
        public int ScaleWidth
        {
            get { return scaleWidth; }
            set
            {
                if (scaleWidth != value)
                {
                    scaleWidth = value;
                    Change("ScaleWidth");
                }
            }
        }

        private int scaleHeight;
        [PluginProperty("Scale Height", "", 0)]
        public int ScaleHeight
        {
            get { return scaleHeight; }
            set
            {
                if (scaleHeight != value)
                {
                    scaleHeight = value;
                    Change("ScaleHeight");
                }
            }
        }

        private float skeletonMaxX;
        [PluginProperty("Skeleton Maximum X", "", 1.0f)]
        public float SkeletonMaxX
        {
            get { return skeletonMaxX; }
            set
            {
                if (skeletonMaxX != value)
                {
                    skeletonMaxX = value;
                    Change("SkeletonMaxX");
                }
            }
        }

        private float skeletonMaxY;
        [PluginProperty("Skeleton Maximum Y", "", 1.0f)]
        public float SkeletonMaxY
        {
            get { return skeletonMaxY; }
            set
            {
                if (skeletonMaxY != value)
                {
                    skeletonMaxY = value;
                    Change("SkeletonMaxY");
                }
            }
        }

        public override void Ready()
        {
            manager = GetManager<KinectManager>();
            manager.Add(this);
            base.Ready();
        }

        public override void Destroy()
        {
            manager.Remove(this);
        }

        public void UpdateJoint(Joint joint)
        {
            float scaledX = 0;
            float scaledY = 0;

            if (ScaleWidth != 0 && ScaleHeight != 0)
            {
                joint = ScaleTo(joint, ScaleWidth, ScaleHeight, SkeletonMaxX, SkeletonMaxY);
                scaledX = joint.Position.X;
                scaledY = joint.Position.Y;
            }
            else
            {
                var point = manager.CoordinateMapper.MapSkeletonPointToColorPoint(joint.Position, ColorImageFormat.RgbResolution640x480Fps30);
                scaledX = point.X * manager.WindowSize.Item1 / 640.0f;
                scaledY = point.Y * manager.WindowSize.Item2 / 480.0f;
            }

            UpdatePosition(scaledX, scaledY);
        }

        private static Joint ScaleTo(Joint joint, int width, int height, float skeletonMaxX, float skeletonMaxY)
        {
            Microsoft.Kinect.SkeletonPoint pos = new SkeletonPoint()
            {
                X = Scale(width, skeletonMaxX, joint.Position.X),
                Y = Scale(height, skeletonMaxY, -joint.Position.Y),
                Z = joint.Position.Z
            };

            joint.Position = pos;

            return joint;
        }


        private static float Scale(int maxPixel, float maxSkeleton, float position)
        {
            float value = ((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel / 2));
            if (value > maxPixel)
                return maxPixel;
            if (value < 0)
                return 0;
            return value;
        }
    }
}
