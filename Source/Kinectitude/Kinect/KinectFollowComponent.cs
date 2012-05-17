using System;
using System.Collections.Generic;
using Kinectitude.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using Kinectitude.Core.Exceptions;
using Kinectitude.Core.Interfaces;
using Microsoft.Kinect;

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

        private float nextDx = 0;
        private float nextDy = 0;

        private TransformComponent transform;
        private IPhysicsComponent physics = null;

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

        public KinectFollowComponent() : base() { }

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
                switch (followType)
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
                switch (followType)
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
            transform = Entity.GetComponent(typeof(TransformComponent)) as TransformComponent;
            if (null == transform)
            {
                List<Type> missing = new List<Type>();
                missing.Add(typeof(TransformComponent));
                throw MissingRequirementsException.MissingRequirement(this, missing);
            }
            physics = Entity.GetComponent(typeof(IPhysicsComponent)) as IPhysicsComponent;
        }

    }
}
