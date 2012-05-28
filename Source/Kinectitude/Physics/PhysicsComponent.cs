using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Kinectitude.Attributes;
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
        public BodyType Bodytype { get; set; }

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

        private bool hasMaximum = false;
        private float maximumVelocity = float.PositiveInfinity;
        [Plugin("Maximum velocity", "")]
        public float MaximumVelocity
        {
            get { return maximumVelocity; }
            set
            {
                hasMaximum = true;
                maximumVelocity = value;
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
            body.Position = new Vector2(tc.X * sizeRatio, tc.Y * sizeRatio);
        }

        public void SetSize()
        {
            throw new NotImplementedException("THE PHYSICS COMPONENT WON'T SET THE SIZE CURRENTLY");
        }

        public override Type ImplementationType
        {
            get { return typeof(APhysicsComponent); }
        }

        public override void Ready()
        {
            pm = GetManager<PhysicsManager>();
            pm.Add(this);

            tc = GetComponent<TransformComponent>();
            tc.SubscribeToX(this, SetPosition);
            tc.SubscribeToY(this, SetPosition);
            tc.SubscribeToHeight(this, SetSize);
            tc.SubscribeToWidth(this, SetSize);

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

            //Set fields on the body
            body.UserData = IEntity;
            body.BodyType = Bodytype;
            body.Restitution = Restitution;
            body.Mass = Mass;
            body.Friction = Friction;
            body.LinearDamping = LinearDamping;
            //TODO Add maximum velocity to body, if applicable

            //Add a listener for collisions
            body.OnCollision += OnCollision;

            SetPosition();
            body.LinearVelocity = new Vector2(xVelocity * speedRatio, yVelocity * speedRatio);
        }

        private bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (crossesLineEvents.Count != 0)
            {
                Body bodyA = fixtureA.Body;
                Body bodyB = fixtureB.Body;

                IDataContainer entityA = bodyA.UserData as IDataContainer;
                IDataContainer entityB = bodyB.UserData as IDataContainer;

                IDataContainer collidedWith = (entityA == IEntity) ? entityB : entityA;

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
