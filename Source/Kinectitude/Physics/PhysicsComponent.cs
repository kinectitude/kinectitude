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
        [PluginProperty("Shape", "")]
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
        [PluginProperty("Restitution", "", 0)]
        public float Restitution
        {
            get { return restitution; }
            set
            {
                if (restitution != value && value > 0)
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
        [PluginProperty("Mass", "", 1.0f)]
        public float Mass
        {
            get { return mass; }
            set
            {
                if (mass != value && value > 0)
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
        [PluginProperty("Friction", "", 0.0f)]
        public float Friction
        {
            get { return friction; }
            set
            {
                if (friction != value && value > 0)
                {
                    friction = value;
                    Change("Friction");
                }
            }
        }


        private float linearDamping = 0;
        [Preset("Bouncy Ball", 0.0)]
        [Preset("Collision Event Line", 0.0)]
        [Preset("Wall", 0.0)]
        [PluginProperty("Linear Damping", "", 0.0f)]
        public float LinearDamping
        {
            get { return linearDamping; }
            set
            {
                if (linearDamping != value && value > 0)
                {
                    linearDamping = value;
                    Change("LinearDamping");
                }
            }
        }

        private float maximumSpeed = float.PositiveInfinity;
        [PluginProperty("Maximum Speed", "", float.PositiveInfinity)]
        public float MaximumSpeed
        {
            get { return maximumSpeed; }
            set
            {
                if (value != maximumSpeed && value > 0)
                {
                    maximumSpeed = value;
                    Change("MaximumSpeed");
                }
            }
        }

        private float minimumSpeed = 0f;
        [PluginProperty("Minimum Speed", "", 0)]
        public float MinimumSpeed
        {
            get { return minimumSpeed; }
            set
            {
                if (value != minimumSpeed && value > 0)
                {
                    minimumSpeed = value;
                    Change("MinimumSpeed");
                }
            }
        }

        private void setBodyVelocity(float velocity)
        {
            hasVelocity |= velocity != 0;
            if (null != body)
            {
                if (velocity != 0 && body.BodyType == BodyType.Static) createBody();
                body.LinearVelocity = new Vector2(XVelocity * speedRatio, YVelocity * speedRatio);
                clampVelocity();
                body.AngularVelocity = AngularVelocity;
            }
        }

        private float xVelocity = 0;
        [PluginProperty("X Velocity", "", 0.0f)]
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
        [PluginProperty("Y Velocity", "", 0.0f)]
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
        [PluginProperty("Angular Velocity", "", 0.0f)]
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

        private bool movesWhenHit = true;
        [Preset("Bouncy Ball", true)]
        [Preset("Collision Event Line", false)]
        [Preset("Wall", false)]
        [PluginProperty("Stationary", "Object moves when hit")]
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
        [PluginProperty("Can Rotate", "Object can rotate as it moves", false)]
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

        private TypeMatcher ignoreCollisionsWith = null;

        [PluginProperty("Ignores Collisions", "Any entities that match this type matcher will pass through this object")]
        public TypeMatcher IgnoreCollisionsWith
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
            tc.Rotation = body.Rotation;

            clampVelocity();

            //no need to use the setters here, they will add extra overhead to things that don't need to be checked
            xVelocity = body.LinearVelocity.X / speedRatio;
            yVelocity = body.LinearVelocity.Y / speedRatio;
            angularVelocity = body.AngularVelocity;
        }

        private void clampVelocity()
        {
            float speed = body.LinearVelocity.Length() / speedRatio;

            if (maximumSpeed == 0 || speed == 0)
            {
                body.LinearVelocity = new Vector2(0f, 0f);
            }
            else
            {
                if (speed > maximumSpeed) body.LinearVelocity = body.LinearVelocity / speed * maximumSpeed;

                if (speed < minimumSpeed)
                {
                    //TODO decide what is good to do here
                    if (0 == speed) body.LinearVelocity = new Vector2(minimumSpeed, 0);
                    else body.LinearVelocity = body.LinearVelocity / speed * minimumSpeed;
                }
            }
        }

        public override void OnUpdate(float t)
        {
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

        public void SetPosition(bool forceUpdate = false)
        {
            if (prevX != tc.X || prevY != tc.Y || body.Rotation != tc.Rotation || forceUpdate)
            {
                //body.Awake = true;
                prevX = tc.X;
                prevY = tc.Y;
                body.Position = new Vector2(prevX * sizeRatio, prevY * sizeRatio);
                body.Rotation = tc.Rotation;
            }
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
        }

        private void createBody()
        {
            if (null != body) pm.PhysicsWorld.RemoveBody(body);

            if ("Ellipse" == Shape)
            {
                float xRadius = ((float)tc.Width / 2.0f) * sizeRatio;
                float yRadius = ((float)tc.Height / 2.0f) * sizeRatio;

                if (xRadius != yRadius)
                {
                    body = BodyFactory.CreateEllipse(pm.PhysicsWorld, xRadius, yRadius, 12, 1f);
                }
                else
                {
                    body = BodyFactory.CreateCircle(pm.PhysicsWorld, xRadius, 1f);
                }
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
                body.Mass = MovesWhenHit ? body.Mass = Mass : (float)int.MaxValue;
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
            body.Restitution = Restitution;
            body.Friction = Friction;
            body.LinearDamping = LinearDamping;
            body.UserData = this;
            //Add a listener for collisions
            body.OnCollision += OnCollision;

            SetPosition(true);
            body.LinearVelocity = new Vector2(xVelocity * speedRatio, yVelocity * speedRatio);
        }

        private bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            Body bodyA = fixtureA.Body;
            Body bodyB = fixtureB.Body;

            PhysicsComponent pcA = bodyA.UserData as PhysicsComponent;
            PhysicsComponent pcB = bodyB.UserData as PhysicsComponent;

            IEntity entityA = pcA.IEntity;
            IEntity entityB = pcB.IEntity;

            IEntity collidedWith = (entityA == IEntity) ? entityB : entityA;

            if (collisionEvents.Count != 0)
            {
                foreach (CollisionEvent ce in collisionEvents)
                {
                    if (ce.CollidesWith.MatchAndSet(collidedWith)) ce.DoActions();
                }
            }

            if (ignoreCollisionsWith != null && ignoreCollisionsWith.MatchAndSet(collidedWith)) return false;

            if (pcA.movesWhenHit == false && pcB.movesWhenHit == false &&
                bodyA.BodyType == BodyType.Dynamic && bodyB.BodyType == BodyType.Dynamic)
            {
                return false;
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
