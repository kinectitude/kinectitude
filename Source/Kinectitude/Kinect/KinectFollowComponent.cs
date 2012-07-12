using Kinectitude.Core.Attributes;
using Kinectitude.Core.ComponentInterfaces;
using Kinectitude.Core.Components;
using Microsoft.Kinect;

namespace Kinectitude.Kinect
{
    [Plugin("Kinect Motion Component", "")]
    public class KinectFollowComponent : KinectComponent
    {
        public enum FollowType { X, Y, Both };

        private float nextDx = 0;
        private float nextDy = 0;

        private TransformComponent transform;
        private IPhysics physics = null;

        private KinectManager manager;

        private JointType joint;
        [Plugin("Joint", "")]
        public JointType Joint 
        {
            get { return joint; }
            set
            {
                if (joint != value)
                {
                    Change("Joint");
                    joint = value;
                }
            }
        }

        private int player;
        [Plugin("Player", "")]
        public int Player
        {
            get { return player; }
            set
            {
                if (player != value)
                {
                    Change("Player");
                    player = value;
                }
            }
        }

        private FollowType direction;
        [Plugin("Axis", "")]
        public FollowType Direction
        {
            get { return direction; }
            set
            {
                if (direction != value)
                {
                    Change("Direction");
                    direction = value;
                }
            }
        }

        public void UpdatePosition(float x, float y)
        {
            float prevx = transform.X;
            nextDx = x - prevx;
            float prevy = transform.Y;
            nextDy = y - prevy;
        }

        public override void OnUpdate(float t)
        {
            //if they are following with physics, we will set a velocity
            if (null != physics)
            {
                switch (Direction)
                {
                    case FollowType.X:
                        physics.XVelocity = nextDx;
                        break;
                    case FollowType.Y:
                        physics.YVelocity = nextDy;
                        break;
                    case FollowType.Both:
                        physics.XVelocity = nextDx;
                        physics.YVelocity = nextDy;
                        break;
                }
            }
            else
            {
                switch (Direction)
                {
                    case FollowType.X:
                        transform.X += nextDx;
                        break;
                    case FollowType.Y:
                        transform.Y += nextDy;
                        break;
                    case FollowType.Both:
                        transform.X += nextDx;
                        transform.Y += nextDy;
                        break;
                }
            }
        }

        public override void Ready()
        {
            manager = GetManager<KinectManager>();
            manager.Add(this);

            transform = GetComponent<TransformComponent>();
            physics = GetComponent<IPhysics>();
        }

        public override void Destroy()
        {
            manager.Remove(this);
        }
    }
}
