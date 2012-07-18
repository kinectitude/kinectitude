using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.ComponentInterfaces;
using Kinectitude.Core.Components;

namespace Kinectitude.Core.AbstractComponents
{
    public enum FollowType { X, Y, Both };

    [Requires(typeof(TransformComponent))]
    public abstract class BasicFollowComponent : Component
    {
        private float nextDx = 0;
        private float nextDy = 0;
        private TransformComponent transform;
        private IPhysics physics = null;

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
            UpdateDelta(x - transform.X, y - transform.Y);
        }

        public void UpdateDelta(float dx, float dy)
        {
            nextDx = dx;
            nextDy = dy;
        }

        public override void OnUpdate(float t)
        {
            //if they are following with physics, we will set a velocity
            if (/*null != physics*/ false)
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
            transform = GetComponent<TransformComponent>();
            physics = GetComponent<IPhysics>();
        }
    }
}
