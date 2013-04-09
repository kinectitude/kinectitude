//-----------------------------------------------------------------------
// <copyright file="BasicFollowComponent.cs" company="Kinectitude">
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
using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.ComponentInterfaces;
using Kinectitude.Core.Components;

namespace Kinectitude.Core.AbstractComponents
{
    [Requires(typeof(TransformComponent))]
    public abstract class BasicFollowComponent : Component
    {
        public enum FollowType { X, Y, Both };

        private float nextDx = 0;
        private float nextDy = 0;
        private TransformComponent transform;
        private IPhysics physics;

        private bool exactMove = false;

        private FollowType direction;
        [PluginProperty("Axis", "", FollowType.Both)]
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
        [PluginProperty("Minimum X", "Minimum X position to follow to", float.NegativeInfinity)]
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
        [PluginProperty("Minimum Y", "Minimum Y position to follow to", float.NegativeInfinity)]
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
        [PluginProperty("Maximum X", "Maximum X position to follow to", float.PositiveInfinity)]
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
        [PluginProperty("Maximum Y", "Maximum Y position to follow to", float.PositiveInfinity)]
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
        [PluginProperty("Ignore Physics", "Ignores physics component, use if motion should not care about physics", false)]
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
            //this is an amount to move.
            exactMove = true;
        }

        public void UpdateDelta(float dx, float dy)
        {
            nextDx = dx;
            nextDy = dy;
            exactMove = false;
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
            nextDx = getNextValue(transform.X, maxXFollow, minXFollow, usePhysics && !exactMove ? nextDx / t : nextDx);
            nextDy = getNextValue(transform.Y, maxYFollow, minYFollow, usePhysics && !exactMove ? nextDy / t : nextDy);

            //if they are following with physics, we will set a velocity
            if (usePhysics)
            {
                switch (Direction)
                {
                    case FollowType.X:
                        physics.XVelocity = nextDx;
                        physics.YVelocity = 0;
                        break;
                    case FollowType.Y:
                        physics.YVelocity = nextDy;
                        physics.XVelocity = 0;
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
