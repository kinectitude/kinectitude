﻿using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.ComponentInterfaces;
using Kinectitude.Core.Components;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;

namespace Kinectitude.Physics
{

    [Plugin("Physics Component", "")]
    [Provides(typeof(IPhysics))]
    [Requires(typeof(TransformComponent))]
    public class PhysicsComponent : Component, IPhysics
    {
        private const float sizeRatio = 1f / 100f;
        private const float speedRatio = 1f / 10f;
        private List<CrossesLineEvent> crossesLineEvents = new List<CrossesLineEvent>();
        private List<CollisionEvent> collisionEvents = new List<CollisionEvent>();
        private Body body;
        private PhysicsManager pm;
        private TransformComponent tc;
 
        [Preset("Bouncy Ball", "circle")]
        [Preset("Collision Event Line", "rectangle")]
        [Preset("Wall", "rectangle")]
        [Plugin("Shape", "")]
        public string Shape { get; set; }

        [Preset("Bouncy Ball", 1.0)]
        [Preset("Collision Event Line", 0.0)]
        [Preset("Wall", 0.0)]
        [Plugin("Restitution", "")]
        public float Restitution { get; set; }

        [Preset("Bouncy Ball", 1.0)]
        [Preset("Collision Event Line", 1.0)]
        [Preset("Wall", 1.0)]
        [Plugin("Mass", "")]
        public float Mass { get; set; }

        [Preset("Bouncy Ball", 0.0)]
        [Preset("Collision Event Line", 0.0)]
        [Preset("Wall", 0.0)]
        [Plugin("Friction", "")]
        public float Friction { get; set; }

        [Preset("Bouncy Ball", 0.0)]
        [Preset("Collision Event Line", 0.0)]
        [Preset("Wall", 0.0)]
        [Plugin("Linear Damping", "")]
        public float LinearDamping { get; set; }

        private float maximumVelocity = float.PositiveInfinity;
        [Plugin("Maximum velocity", "")]
        public float MaximumVelocity
        {
            get { return maximumVelocity; }
            set
            {
                maximumVelocity = value;
            }
        }

        private float minimumVelocity = 0;
        [Plugin("Minimum velocity", "")]
        public float MinimumVelocity
        {
            get { return minimumVelocity; }
            set
            {
                minimumVelocity = value;
            }
        }

        private float xVelocity;
        [Plugin("X Velocity", "")]
        public float XVelocity
        {
            get { return xVelocity; }
            set 
            {
                if (null != body)
                {
                    body.LinearVelocity = new Vector2(value * speedRatio, YVelocity * speedRatio);
                }
                xVelocity = value;
                hasPotentialVelocity = true;
            }
        }

        private float yVelocity;
        [Plugin("Y Velocity", "")]
        public float YVelocity
        {
            get { return yVelocity; }
            set
            {
                if (null != body)
                {
                    body.LinearVelocity = new Vector2(xVelocity * speedRatio, value * speedRatio);
                }
                yVelocity = value;
                hasPotentialVelocity = true;
            }
        }

        private float angularVelocity;
        [Plugin("Angular Velocity Velocity", "")]
        public float AngularVelocity
        {
            get { return angularVelocity; }
            set
            {
                angularVelocity = value;
                hasPotentialVelocity = true;
            }
        }

        [Preset("Bouncy Ball", true)]
        [Preset("Collision Event Line", false)]
        [Preset("Wall", false)]
        [Plugin("Object moves when hit", "")]
        public bool MovesWhenHit { get; set; }

        [Preset("Bouncy Ball", false)]
        [Preset("Collision Event Line", false)]
        [Preset("Wall", false)]
        [Plugin("Object can rotate as it moves", "")]
        public bool FixedRotation { get; set; }

        private bool hasCollisions = false;
        private bool hasPotentialVelocity = false;
        private float prevX;
        private float prevY;

        public PhysicsComponent()
        {
            MovesWhenHit = true;
            FixedRotation = true;
        }

        private void checkCrossesLine(float x, float y)
        {
            //TODO make this done in farseer somehow
            if (crossesLineEvents.Count != 0)
            {
                foreach (CrossesLineEvent evt in crossesLineEvents)
                {
                    double cross = evt.Location;
                    //from neg and positive
                    bool neg = true;
                    bool pos = true;
                    if (CrossesLineEvent.FromDirection.Negative == evt.Direction)
                    {
                        pos = false;
                    }
                    else if (CrossesLineEvent.FromDirection.Positive == evt.Direction)
                    {
                        neg = false;
                    }

                    double prev;
                    double next;
                    if (CrossesLineEvent.LineType.X == evt.Line)
                    {
                        prev = prevX;
                        next = x;
                    }
                    else
                    {
                        prev = prevY;
                        next = y;
                    }
                    if (pos && 0 < next - prev && cross < next && cross >= prev)
                    {
                        evt.DoActions();
                    }
                    else if (neg && 0 > next - prev && cross > next && cross <= prev)
                    {
                        evt.DoActions();
                    }
                }
            }
        }

        private void setVelocity()
        {
            //the body needs to be moved because of a set position that was triggered
            if (prevX != tc.X || prevY != tc.Y)
            {

            }
            else
            {
                float x = body.Position.X / sizeRatio;
                float y = body.Position.Y / sizeRatio;
                tc.X = x;
                tc.Y = y;
                checkCrossesLine(x, y);
            }
			
			float speed = body.LinearVelocity.Length();

            xVelocity = body.LinearVelocity.X / speedRatio;
            yVelocity = body.LinearVelocity.Y / speedRatio;

            if (speed > maximumVelocity)
            {
                body.LinearVelocity = body.LinearVelocity / speed * maximumVelocity;
            }

            if (speed < minimumVelocity)
            {
                body.LinearVelocity = body.LinearVelocity / speed * minimumVelocity;
            }

            xVelocity = body.LinearVelocity.X / speedRatio;
            yVelocity = body.LinearVelocity.Y / speedRatio;
        }

        public override void OnUpdate(float t)
        {
            //the body needs to be moved because of a set position that was triggered
            if (prevX == tc.X && prevY == tc.Y)
            {
                float x = body.Position.X / sizeRatio;
                float y = body.Position.Y / sizeRatio;
                tc.X = x;
                tc.Y = y;
                checkCrossesLine(x, y);
            }

            setVelocity();
        }

        public void AddCrossLineEvent(CrossesLineEvent evt)
        {
            crossesLineEvents.Add(evt);
        }

        public void AddCollisionEvent(CollisionEvent evt)
        {
            hasCollisions = true;
            collisionEvents.Add(evt);
        }

        public void SetPosition()
        {
            body.Awake = true;
            prevX = tc.X;
            prevY = tc.Y;
            body.Position = new Vector2(prevX * sizeRatio, prevY * sizeRatio);
        }

        public void SetSize()
        {
            //Vertices vertices =
            throw new NotImplementedException("THE PHYSICS COMPONENT WON'T SET THE SIZE CURRENTLY");
        }

        public override void Ready()
        {
            pm = GetManager<PhysicsManager>();
            pm.Add(this);

            tc = GetComponent<TransformComponent>();

            if ("circle" == Shape)
            {
                body = BodyFactory.CreateCircle(pm.PhysicsWorld, tc.Width * sizeRatio, 1f);
            }
            else
            {
                float width = tc.Width * sizeRatio;
                float height = tc.Height * sizeRatio;
                body = BodyFactory.CreateRectangle(pm.PhysicsWorld, width, height, 1f);
            }

            if (hasCollisions)
            {
                body.BodyType = BodyType.Dynamic;
            }
            else if (hasPotentialVelocity)
            {
                body.BodyType = BodyType.Kinematic;
            }
            else
            {
                body.BodyType = BodyType.Static;
            }

            body.FixedRotation = FixedRotation;
            body.AngularVelocity = AngularVelocity;
            body.Mass = MovesWhenHit ? body.Mass = Mass : (float)int.MaxValue;
            body.Restitution = Restitution;
            body.Friction = Friction;
            body.LinearDamping = LinearDamping;
            body.UserData = IEntity;
            
            //Add a listener for collisions
            body.OnCollision += OnCollision;

            SetPosition();
            body.LinearVelocity = new Vector2(xVelocity * speedRatio, yVelocity * speedRatio);
        }

        private bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            Body bodyA = fixtureA.Body;
            Body bodyB = fixtureB.Body;

            IEntity entityA = bodyA.UserData as IEntity;
            IEntity entityB = bodyB.UserData as IEntity;

            IEntity collidedWith = (entityA == IEntity) ? entityB : entityA;

            if (collisionEvents.Count != 0)
            {
                foreach (CollisionEvent ce in collisionEvents)
                {
                    if (ce.CollidesWith.MatchAndSet(collidedWith))
                    {
                        ce.DoActions();
                    }
                }
            }

            //Allow the collison to occur
            return true;
        }

        public override void Destroy()
        {
            pm.Remove(this);
            pm.PhysicsWorld.RemoveBody(body);
        }
    }
}
