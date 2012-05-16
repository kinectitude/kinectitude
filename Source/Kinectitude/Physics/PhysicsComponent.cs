using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using Kinectitude.Core;
using Kinectitude.Core.Interfaces;
using Kinectitude.Core.Components;
using Kinectitude.Attributes;
using Kinectitude.Core.Base;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;
using Kinectitude.Core.Exceptions;

namespace Kinectitude.Physics
{
    [Plugin("Physics Component", "")]
    public class PhysicsComponent : Component, IPhysicsComponent
    {
        private const float sizeRatio = 1f / 100f;
        private const float speedRatio = 1f / 10f;
        private List<CrossesLineEvent> crossesLineEvents = new List<CrossesLineEvent>();
        private Body body;
        private PhysicsManager pm;
        private TransformComponent tc;

        [Plugin("Shape", "")]
        public string Shape { get; set; }

        private float xVelocity;
        [Plugin("Horizontal Velocity", "")]
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
            }
        }

        private float yVelocity;
        [Plugin("Vertical Velocity", "")]
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
            }
        }

        [Plugin("Angular Velocity", "")]
        public float AngularVelocity { get; set; }

        override public Type ManagerType(){return typeof(PhysicsManager);}

        public PhysicsComponent(Entity entity) : base(entity) { }

        public override void OnUpdate(float t)
        {
            float prevX = tc.X;
            float prevY = tc.Y;
            float x = body.Position.X / sizeRatio;
            float y = body.Position.Y / sizeRatio;

            tc.setX(this, x);
            tc.setY(this, y);

            //TODO this is a hack
            int Height = tc.Height;
            if (y < 0 || y > 600 - Height)
            {
                body.LinearVelocity = new Vector2(body.LinearVelocity.X, -body.LinearVelocity.Y);
            }

            xVelocity = body.LinearVelocity.X / speedRatio;
            yVelocity = body.LinearVelocity.Y / speedRatio;

            //TODO make this done in farseer somehow
            if (crossesLineEvents.Count != 0)
            {
                foreach (CrossesLineEvent evt in crossesLineEvents)
                {
                    double cross = evt.Position;
                    //from neg and positive
                    bool neg = true;
                    bool pos = true;
                    if (CrossesLineEvent.FromDirection.Negative == evt.From)
                    {
                        pos = false;
                    }
                    else if (CrossesLineEvent.FromDirection.Positive == evt.From)
                    {
                        neg = false;
                    }

                    double prev;
                    double next;
                    if (CrossesLineEvent.LineType.X == evt.Cross)
                    {
                        prev = prevX;
                        next = x;
                    }
                    else
                    {
                        prev = prevY;
                        next = y;
                    }
                    if (pos && 0 < next - prev  && cross < next  && cross >= prev)
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

        public void AddCrossLineEvent(CrossesLineEvent evt)
        {
            crossesLineEvents.Add(evt);
        }

        public void setPosition()
        {
            body.Position = new Vector2(tc.X * sizeRatio, tc.Y * sizeRatio);
        }

        public void setSize()
        {
            throw new NotImplementedException("THE PHYSICS COMPONENT WON'T SET THE SIZE CURRENTLY");
        }

        public void Initialize(PhysicsManager pm)
        {   
            this.pm = pm;
        }

        public override Type ImplementationType()
        {
            return typeof(IPhysicsComponent);
        }

        public override void Ready()
        {
            List<Type> missing = new List<Type>();
            tc = Entity.GetComponent(typeof(TransformComponent)) as TransformComponent;
            if (null == tc)
            {
                missing.Add(typeof(TransformComponent));
                throw new MissingRequirementsException(this, missing);
            }
            tc.SubscribeToX(this, setPosition);
            tc.SubscribeToY(this, setPosition);
            tc.SubscribeToHeight(this, setSize);
            tc.SubscribeToWidht(this, setSize);
            //TODO this is temporary
            if ("circle" == Shape)
            {
                body = BodyFactory.CreateCircle(pm.PhysicsWorld, tc.Width * sizeRatio, 1f);
                //TODO this is a hack
                body.Restitution = 1f;
                body.BodyType = BodyType.Dynamic;
            }
            else
            {
                float width = tc.Width * sizeRatio;
                float height = tc.Height * sizeRatio;
                body = BodyFactory.CreateRectangle(pm.PhysicsWorld, width, height, 1f);
                //TODO this is a hack
                body.Restitution = 0f;
                body.BodyType = BodyType.Kinematic;
            }
            body.Mass = 1f;
            body.Friction = 0f;
            body.LinearDamping = 0f;
            setPosition();
            body.LinearVelocity = new Vector2(xVelocity * speedRatio, yVelocity * speedRatio);
        }
    }
}
