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
                    direction = value;
                    Change("Direction");
                }
            }
        }

        private float minXFollow = float.NegativeInfinity;
        [Plugin("Minimum X position to follow to","")]
        public float MinXFollow
        {
            get {return minXFollow;}
            set 
            {
                if (minXFollow != value)
                {
                    minXFollow = value;
                    Change("MinXFollow");
                }
            }
        }

        private float minYFollow = float.NegativeInfinity;
        [Plugin("Minimum Y position to follow to", "")]
        public float MinYFollow
        {
            get { return minYFollow; }
            set
            {
                if (minYFollow != value)
                {
                    minYFollow = value;
                    Change("MinYFollow");
                }
            }
        }

        private float maxXFollow = float.PositiveInfinity;
        [Plugin("Maximum X position to follow to", "")]
        public float MaxXFollow
        {
            get { return maxXFollow; }
            set
            {
                if (maxXFollow != value)
                {
                    maxXFollow = value;
                    Change("MaxXFollow");
                }
            }
        }

        private float maxYFollow = float.PositiveInfinity;
        [Plugin("Maximum Y position to follow to", "")]
        public float MaxYFollow
        {
            get { return maxYFollow; }
            set
            {
                if (maxYFollow != value)
                {
                    maxYFollow = value;
                    Change("MaxYFollow");
                }
            }
        }

        private bool ignoresPhysics = false;
        [Plugin("Ignore Physics", "Ignores physics component, use if motion should not care about physics")]
        public bool IgnoresPhysics
        {
            get { return ignoresPhysics; }
            set
            {
                if (value != ignoresPhysics)
                {
                    ignoresPhysics = value;
                    Change("IgnoresPhysics");
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

        private float getNextValue(float position, float max, float min, float velocity)
        {
            if (velocity == 0) return 0;
            if (position < min && velocity < 0 || position > max && velocity > 0) return 0;
            if (position + velocity < min) return min - position;
            if (position + velocity > max) return max - position;
            return velocity;
        }

        public override void OnUpdate(float t)
        {
            bool usePhysics = null != physics && ignoresPhysics == false;
            nextDx = getNextValue(transform.X, maxXFollow, minXFollow, usePhysics?nextDx/t:nextDx);
            nextDy = getNextValue(transform.Y, maxYFollow, minYFollow, usePhysics?nextDy/t:nextDy);

            //if they are following with physics, we will set a velocity
            if (usePhysics)
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
