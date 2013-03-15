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
    }
}
