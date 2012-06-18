using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.ComponentInterfaces;
using Kinectitude.Core.Components;
using Microsoft.Xna.Framework;

namespace Kinectitude.Physics
{

    [Plugin("Physics Component", "")]
    public class PhysicsComponent : APhysicsComponent
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

        [Preset("Bouncy Ball", BodyType.Dynamic)]
        [Preset("Collision Event Line", BodyType.Kinematic)]
        [Preset("Wall", BodyType.Kinematic)]
        [Plugin("Body Type", "")]
        public BodyType BodyType { get; set; }

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

        private bool hasMaximumVelocity = false;
        private float maximumVelocity = float.PositiveInfinity;
        [Plugin("Maximum velocity", "")]
        public float MaximumVelocity
        {
            get { return maximumVelocity; }
            set
            {
                hasMaximumVelocity = true;
                maximumVelocity = value;
            }
        }

        private bool hasMinimumVelocity = false;
        private float minimumVelocity = 0;
        [Plugin("Minimum velocity", "")]
        public float MinimumVelocity
        {
            get { return minimumVelocity; }
            set
            {
                hasMinimumVelocity = true;
                minimumVelocity = value;
            }
        }

        private float xVelocity;
        public override float XVelocity
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

        public override float YVelocity
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

        public override float AngularVelocity { get; set; }

        public PhysicsComponent() : base() { }

        public override void OnUpdate(float t)
        {
            float prevX = tc.X;
            float prevY = tc.Y;
            float x = body.Position.X / sizeRatio;
            float y = body.Position.Y / sizeRatio;

            tc.setX(this, x);
            tc.setY(this, y);

            xVelocity = body.LinearVelocity.X / speedRatio;
            yVelocity = body.LinearVelocity.Y / speedRatio;
			
			float speed = body.LinearVelocity.Length();

            if (speed > maximumVelocity)
            {
                body.LinearVelocity = body.LinearVelocity / speed * maximumVelocity;
            }

            if (speed < minimumVelocity)
            {
                body.LinearVelocity = body.LinearVelocity / speed * minimumVelocity;
            }

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

        public void AddCrossLineEvent(CrossesLineEvent evt)
        {
            crossesLineEvents.Add(evt);
        }

        public void AddCollisionEvent(CollisionEvent evt)
        {
            collisionEvents.Add(evt);
        }

        public void SetPosition()
        {
			body.Awake = true;
            body.Position = new Vector2(tc.X * sizeRatio, tc.Y * sizeRatio);
        }

        public void SetSize()
        {
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

            body.BodyType = BodyType.Dynamic;
            body.Restitution = Restitution;
            body.Friction = Friction;
            body.LinearDamping = LinearDamping;
            body.UserData = IEntity;

            if (BodyType == BodyType.Kinematic || BodyType == BodyType.Static)
            {
                body.Mass = (float)int.MaxValue;
            }
            else
            {
                body.Mass = Mass;
            }

            body.IsStatic = (BodyType == BodyType.Static);
            body.Awake = false;

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
