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
using FarseerPhysics.Common;
using Kinectitude.Core.Data;

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

        private string shape = "rectangle";
        [Preset("Bouncy Ball", "circle")]
        [Preset("Collision Event Line", "rectangle")]
        [Preset("Wall", "rectangle")]
        [Plugin("Shape", "")]
        public string Shape 
        {
            get { return shape; }
            set
            {
                if (shape != value)
                {
                    shape = value;
                    Change("Shape");
                }
            }
        }

        private float restitution = 0;
        [Preset("Bouncy Ball", 1.0)]
        [Preset("Collision Event Line", 0.0)]
        [Preset("Wall", 0.0)]
        [Plugin("Restitution", "")]
        public float Restitution
        {
            get { return restitution; }
            set
            {
                if (restitution != value)
                {
                    restitution = value;
                    Change("Restitution");
                }
            }
        }


        private float mass = 1;
        [Preset("Bouncy Ball", 1.0)]
        [Preset("Collision Event Line", 1.0)]
        [Preset("Wall", 1.0)]
        [Plugin("Mass", "")]
        public float Mass
        {
            get { return mass; }
            set
            {
                if (mass != value)
                {
                    mass = value;
                    Change("Mass");
                }
            }
        }


        private float friction = 0;
        [Preset("Bouncy Ball", 0.0)]
        [Preset("Collision Event Line", 0.0)]
        [Preset("Wall", 0.0)]
        [Plugin("Friction", "")]
        public float Friction
        {
            get { return friction; }
            set
            {
                if (friction != value)
                {
                    friction = value;
                    Change("Friction");
                }
            }
        }


        private float linearDampining = 0;
        [Preset("Bouncy Ball", 0.0)]
        [Preset("Collision Event Line", 0.0)]
        [Preset("Wall", 0.0)]
        [Plugin("Linear Damping", "")]
        public float LinearDamping
        {
            get { return linearDampining; }
            set
            {
                if (linearDampining != value)
                {
                    linearDampining = value;
                    Change("LinearDampining");
                }
            }
        }

        private float maximumVelocity = float.PositiveInfinity;
        [Plugin("Maximum velocity", "")]
        public float MaximumVelocity
        {
            get { return maximumVelocity; }
            set
            {
                if (value != maximumVelocity)
                {
                    maximumVelocity = value;
                    Change("MaximumVelocity");
                }
            }
        }

        private float minimumVelocity = 0;
        [Plugin("Minimum velocity", "")]
        public float MinimumVelocity
        {
            get { return minimumVelocity; }
            set
            {
                if (value != minimumVelocity)
                {
                    minimumVelocity = value;
                    Change("MinimumVelocity");
                }
            }
        }

        private void setBodyVelocity(float velocity)
        {
            hasVelocity |= velocity != 0;
            if (null != body)
            {
                if (velocity != 0 && body.BodyType == BodyType.Static)
                {
                    pm.PhysicsWorld.RemoveBody(body);
                    createBody();
                }
                body.LinearVelocity = new Vector2(XVelocity * speedRatio, YVelocity * speedRatio);
                body.AngularVelocity = AngularVelocity;
            }
        }

        private float xVelocity = 0;
        [Plugin("X Velocity", "")]
        public float XVelocity
        {
            get { return xVelocity; }
            set 
            {
                if (value != xVelocity)
                {
                    xVelocity = value;
                    setBodyVelocity(value);
                    Change("XVelocity");
                }
            }
        }

        private float yVelocity = 0;
        [Plugin("Y Velocity", "")]
        public float YVelocity
        {
            get { return yVelocity; }
            set
            {
                if (value != yVelocity)
                {
                    yVelocity = value;
                    setBodyVelocity(value);
                    Change("YVelocity");
                }
            }
        }

        private float angularVelocity = 0;
        [Plugin("Angular Velocity Velocity", "")]
        public float AngularVelocity
        {
            get { return angularVelocity; }
            set
            {
                if (value != angularVelocity)
                {
                    angularVelocity = value;
                    setBodyVelocity(value);
                    Change("AngularVelocity");
                }
            }
        }

        private bool movesWhenHit;
        [Preset("Bouncy Ball", true)]
        [Preset("Collision Event Line", false)]
        [Preset("Wall", false)]
        [Plugin("Object moves when hit", "")]
        public bool MovesWhenHit {
            get { return movesWhenHit; }
            set
            {
                if (value != movesWhenHit)
                {
                    movesWhenHit = value;
                    if (body != null && movesWhenHit && body.BodyType != BodyType.Dynamic) createBody();
                    Change("MovesWhenHit");
                }
            }
 
        }

        private bool fixedRotation = false;
        [Preset("Bouncy Ball", true)]
        [Preset("Collision Event Line", false)]
        [Preset("Wall", false)]
        [Plugin("Object can rotate as it moves", "")]
        public bool FixedRotation
        {
            get { return fixedRotation; }
            set
            {
                if (value != fixedRotation)
                {
                    fixedRotation = value;
                    Change("FixedRotation");
                }
            }
        }

        private ITypeMatcher ignoreCollisionsWith = null;

        [Plugin("Ignores collisions with", "Any entities that match this type matcher will pass through this object")]
        public ITypeMatcher IgnoreCollisionsWith
        {
            get { return ignoreCollisionsWith; }
            set
            {
                if (ignoreCollisionsWith != value)
                {
                    //TODO override = operator for TypeMatcher
                    ignoreCollisionsWith = value;
                    Change("IgnoreCollisionsWith");
                }
            }
        }

        private bool hasCollisions = false;
        private bool hasVelocity = false;
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
            if (prevX == tc.X && prevY == tc.Y)
            {
                float x = body.Position.X / sizeRatio;
                float y = body.Position.Y / sizeRatio;
                tc.X = x;
                tc.Y = y;
                checkCrossesLine(x, y);
            }

            tc.Rotation = body.Rotation;

			float speed = body.LinearVelocity.Length() / speedRatio;

            if (speed > maximumVelocity)
            {
                body.LinearVelocity = body.LinearVelocity / speed * maximumVelocity;
            }

            if (speed < minimumVelocity)
            {
                body.LinearVelocity = body.LinearVelocity / speed * minimumVelocity;
            }

            //no need to use the setters here, they will add extra overhead to things that don't need to be checked
            xVelocity = body.LinearVelocity.X / speedRatio;
            yVelocity = body.LinearVelocity.Y / speedRatio;
            angularVelocity = body.AngularVelocity;
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
            body.Rotation = tc.Rotation;
            body.Position = new Vector2(prevX * sizeRatio, prevY * sizeRatio);
        }

        public void SetSize()
        {
            createBody();
        }

        public override void Ready()
        {
            pm = GetManager<PhysicsManager>();
            pm.Add(this);

            tc = GetComponent<TransformComponent>();

            createBody();
            
            //Add a listener for collisions
            body.OnCollision += OnCollision;

            SetPosition();
            body.LinearVelocity = new Vector2(xVelocity * speedRatio, yVelocity * speedRatio);
        }

        private void createBody()
        {
            if ("Ellipse" == Shape)
            {
                float xRadius = ((float)tc.Width / 2.0f) * sizeRatio;
                float yRadius = ((float)tc.Height / 2.0f) * sizeRatio;

                //Using 5000 vertices for the ellipse for now.
                body = BodyFactory.CreateEllipse(pm.PhysicsWorld, xRadius, yRadius, 5000, 1f);
            }
            else
            {
                float width = (float)tc.Width * sizeRatio;
                float height = (float)tc.Height * sizeRatio;
                body = BodyFactory.CreateRectangle(pm.PhysicsWorld, width, height, 1f);
            }

            if (hasCollisions)
            {
                body.BodyType = BodyType.Dynamic;
            }
            else if (hasVelocity)
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

            if (ignoreCollisionsWith != null && ignoreCollisionsWith.MatchAndSet(collidedWith)) return false;

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
