using Microsoft.Kinect;
using System.Xml;
using Kinectitude.Core;
using System;
using Kinectitude.Attributes;

namespace Kinectitude.Kinect
{
    [Plugin("Kinect Motion Component", "")]
    public class KinectFollowComponent:KinectComponent
    {
        public enum PlayerType { P1, P2};
        public enum FollowType { X, Y, Both };

        public PlayerType followPlayer;
        public JointType followJoint;
        private FollowType followType;

        private double nextDx = 0;
        private double nextDy = 0;

        [Plugin("Joint", "")]
        public string Joint
        {
            set
            {
                switch (value.ToUpper())
                {
                    case ("HIPCENTER"):
                        followJoint = JointType.HipCenter;
                        break;
                    case ("SPINE"):
                        followJoint = JointType.Spine;
                        break;
                    case ("SHOULDERCENTER"):
                        followJoint = JointType.ShoulderCenter;
                        break;
                    case ("HEAD"):
                        followJoint = JointType.Head;
                        break;
                    case ("SHOULDERLEFT"):
                        followJoint = JointType.ShoulderLeft;
                        break;
                    case ("FOOTRIGHT"):
                        followJoint = JointType.FootRight;
                        break;
                    case ("ANKLERIGHT"):
                        followJoint = JointType.AnkleRight;
                        break;
                    case ("KNEERIGHT"):
                        followJoint = JointType.KneeRight;
                        break;
                    case ("HIPRIGHT"):
                        followJoint = JointType.HipRight;
                        break;
                    case ("FOOTLEFT"):
                        followJoint = JointType.FootLeft;
                        break;
                    case ("ANKLELEFT"):
                        followJoint = JointType.AnkleLeft;
                        break;
                    case ("KNEELEFT"):
                        followJoint = JointType.KneeLeft;
                        break;
                    case ("HIPLEFT"):
                        followJoint = JointType.HipLeft;
                        break;
                    case ("HANDRIGHT"):
                        followJoint = JointType.HandRight;
                        break;
                    case ("WRISTRIGHT"):
                        followJoint = JointType.WristRight;
                        break;
                    case ("ELBORIGHT"):
                        followJoint = JointType.ElbowRight;
                        break;
                    case ("SNOULDERRIGHT"):
                        followJoint = JointType.ShoulderRight;
                        break;
                    case ("HANDLEFT"):
                        followJoint = JointType.HandLeft;
                        break;
                    case ("WRISTLEFT"):
                        followJoint = JointType.WristLeft;
                        break;
                    case ("ELBOWLEFT"):
                        followJoint = JointType.ElbowLeft;
                        break;
                    default:
                        break;
                    //TODO
                }
            } 
        }

        [Plugin("Player", "")]
        public string Player
        {
            set
            {
                switch (value)
                {
                    case "P1":
                        followPlayer = PlayerType.P1;
                        break;
                    case "P2":
                        followPlayer = PlayerType.P2;
                        break;
                    default:
                        break;
                    //TODO some error
                }
            }
        }

        [Plugin("Axis", "")]
        public string Direction
        {
            set
            {
                switch (value.ToLower())
                {
                    case ("x"):
                        followType = FollowType.X;
                        break;
                    case ("y"):
                        followType = FollowType.Y;
                        break;
                    case ("both"):
                        followType = FollowType.Both;
                        break;
                    default:
                        //TODO
                        break;
                }
            }
        }

        public KinectFollowComponent(Entity entity) : base(entity) { }

        public void UpdatePosition(float x, float y)
        {
            if (FollowType.Both == followType)
            {
                double prevx = double.Parse(Entity["x"]);
                double prevy = double.Parse(Entity["y"]);
                nextDx = x - prevx;
                nextDy = y - prevy;
            }
            else if (FollowType.X == followType)
            {
                double prevx = double.Parse(Entity["x"]);
                nextDx = x - prevx;
            }
            else
            {
                double prevy = double.Parse(Entity["y"]);
                nextDy = y - prevy;
            }
        }

        public override void OnUpdate(double t)
        {
            if (FollowType.Both == followType)
            {
                Entity["Dx"] = nextDx.ToString();
                Entity["Dy"] = nextDy.ToString();
                nextDx = nextDy = 0;
            }
            else if (FollowType.X == followType)
            {
                Entity["Dx"] = nextDy.ToString();
                nextDx = 0;
            }
            else
            {
                Entity["Dy"] = nextDy.ToString();
                nextDy = 0;
            }
        }
    }
}
