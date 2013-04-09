//-----------------------------------------------------------------------
// <copyright file="PhysicsManager.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using FarseerPhysics.Dynamics;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Microsoft.Xna.Framework;
using System;

namespace Kinectitude.Physics
{
    [Plugin("Physics Manager", "")]
    public class PhysicsManager : Manager<PhysicsComponent>
    {
        private const float DistanceRatio = 1f / 100f;
        private const float VelocityRatio = 1f / 10f;

        public static float ConvertDistanceToFarseer(float value)
        {
            return value * DistanceRatio;
        }

        public static float ConvertDistanceToGame(float value)
        {
            return value / DistanceRatio;
        }
        
        public static float ConvertVelocityToFarseer(float velocity)
        {
            return velocity * VelocityRatio;
        }

        public static float ConvertVelocityToGame(float velocity)
        {
            return velocity / VelocityRatio;
        }

        public static float DegreesToRadians(float degrees)
        {
            return degrees * (float)Math.PI / 180.0f;
        }

        public static float RadiansToDegrees(float radians)
        {
            return radians * 180.0f / (float)Math.PI;
        }

        [PluginProperty("Y Gravity", "How fast things are pulled down")]
        public float YGravity 
        {
            get { return PhysicsWorld.Gravity.Y; }
            set
            {
                if (PhysicsWorld.Gravity.Y != value)
                {
                    PhysicsWorld.Gravity = new Vector2(PhysicsWorld.Gravity.X, value);
                    PhysicsWorld.ClearForces();
                    Change("YGravity");
                }
            }
        }

        [PluginProperty("X Gravity", "How fast things are pulled to the left")]
        public float XGravity 
        {
            get { return PhysicsWorld.Gravity.X; }
            set
            {
                if (PhysicsWorld.Gravity.X != value)
                {
                    PhysicsWorld.Gravity = new Vector2(value, PhysicsWorld.Gravity.Y);
                    PhysicsWorld.ClearForces();
                    Change("XGravity");
                }
            }
        }

        public World PhysicsWorld { get; private set; }

        public PhysicsManager()
        {
            PhysicsWorld = new World(Vector2.Zero);
            PhysicsWorld.ClearForces();
        }

        public override void OnUpdate(float t)
        {
            PhysicsWorld.Step(t);
			
            foreach (PhysicsComponent pc in Children)
            {
                pc.OnUpdate(t);
            }
        }
    }
}
