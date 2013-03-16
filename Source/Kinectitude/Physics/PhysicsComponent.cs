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

    [Plugin("Physics Component", ""), Provides(typeof(IPhysics)), Requires(typeof(TransformComponent))]
    public class PhysicsComponent : Component, IPhysics
    {
        private sealed class PhysicsTransform : ITransform
        {
            private readonly ITransform lastTransform;
            private float prevX;
            private float prevY;
            private float prevRotation;
            private Body body;

            public float PreviousX
            {
                get { return prevX; }
            }

            public float PreviousY
            {
                get { return prevY; }
            }

            public float X
            {
                get { return PhysicsManager.ConvertDistanceToGame(body.Position.X); }
                set
                {
                    value = PhysicsManager.ConvertDistanceToFarseer(value);
                    if (body.Position.X != value)
                    {
                        body.Position = new Vector2(value, body.Position.Y);
                        Change("X");
                    }
                }
            }

            public float Y
            {
                get { return PhysicsManager.ConvertDistanceToGame(body.Position.Y); }
                set
                {
                    value = PhysicsManager.ConvertDistanceToFarseer(value);
                    if (body.Position.Y != value)
                    {
                        body.Position = new Vector2(body.Position.X, value);
                        Change("Y");
                    }
                }
            }

            public float Width { get; set; }

            public float Height { get; set; }

            public float Rotation
            {
                get { return PhysicsManager.RadiansToDegrees(body.Rotation); }
                set
                {
                    value = PhysicsManager.DegreesToRadians(value);
                    if (body.Rotation != value)
                    {
                        body.Rotation = value;
                        Change("Rotation");
                    }
                }
            }

            public Body Body
            {
                set
                {
                    if (null == body)
                    {
                        body = value;

                        X = lastTransform.X;
                        Y = lastTransform.Y;
                        Rotation = lastTransform.Rotation;
                    }
                }
            }

            public event ChangeEventHandler Changed;

            public PhysicsTransform(ITransform lastTransform)
            {
                this.lastTransform = lastTransform;

                Width = lastTransform.Width;
                Height = lastTransform.Height;
            }

            private void Change(string property)
            {
                if (null != Changed)
                {
                    Changed(property);
                }
            }

            public void Update()
            {
                if (body.Awake)
                {
                    if (X != prevX)
                    {
                        Change("X");
                    }

                    if (Y != prevY)
                    {
                        Change("Y");
                    }

                    if (Rotation != prevRotation)
                    {
                        Change("Rotation");
                    }

                    prevX = X;
                    prevY = Y;
                    prevRotation = Rotation;
                }
            }
        }

        public enum ShapeType { Rectangle, Ellipse }

        private struct BodyDefinition
        {
            public ShapeType Shape;
            public float Restitution;
            public float Mass;
            public float Friction;
            public float LinearDamping;
            public float XVelocity;
            public float YVelocity;
            public float Speed;
            public float AngularVelocity;
            public Kinectitude.Core.ComponentInterfaces.BodyType BodyType;
            public bool FixedRotation;
            public TypeMatcher IgnoreCollisionsWith;
        }

        private FarseerPhysics.Dynamics.BodyType ConvertBodyTypeToFarseer(Kinectitude.Core.ComponentInterfaces.BodyType type)
        {
            switch (type)
            {
                case Core.ComponentInterfaces.BodyType.Dynamic: return FarseerPhysics.Dynamics.BodyType.Dynamic;
                case Core.ComponentInterfaces.BodyType.Kinematic: return FarseerPhysics.Dynamics.BodyType.Kinematic;
                case Core.ComponentInterfaces.BodyType.Static: return FarseerPhysics.Dynamics.BodyType.Static;
            }

            return default(FarseerPhysics.Dynamics.BodyType);
        }

        private Kinectitude.Core.ComponentInterfaces.BodyType ConvertBodyTypeToGame(FarseerPhysics.Dynamics.BodyType type)
        {
            switch (type)
            {
                case FarseerPhysics.Dynamics.BodyType.Dynamic: return Core.ComponentInterfaces.BodyType.Dynamic;
                case FarseerPhysics.Dynamics.BodyType.Kinematic: return Core.ComponentInterfaces.BodyType.Kinematic;
                case FarseerPhysics.Dynamics.BodyType.Static: return Core.ComponentInterfaces.BodyType.Static;
            }

            return default(Kinectitude.Core.ComponentInterfaces.BodyType);
        }

        private readonly List<CrossesLineEvent> crossesLineEvents = new List<CrossesLineEvent>();
        private readonly List<CollisionEvent> collisionEvents = new List<CollisionEvent>();
        
        private Body body;
        private PhysicsManager manager;
        private PhysicsTransform transform;
        private BodyDefinition def;

        [PluginProperty("Shape", "", ShapeType.Rectangle)]
        public ShapeType Shape 
        {
            get { return def.Shape; }
            set
            {
                if (def.Shape != value)
                {
                    def.Shape = value;
                    Change("Shape");
                }
            }
        }
        
        [PluginProperty("Restitution", "", 0)]
        public float Restitution
        {
            get { return def.Restitution; }
            set
            {
                if (def.Restitution != value)
                {
                    def.Restitution = value;
                    if (null != body) body.Restitution = value;
                    Change("Restitution");
                }
            }
        }

        [PluginProperty("Mass", "", 1.0f)]
        public float Mass
        {
            get { return def.Mass; }
            set
            {
                if (def.Mass != value)
                {
                    def.Mass = value;
                    if (null != body) body.Mass = value;
                    Change("Mass");
                }
            }
        }

        [PluginProperty("Friction", "", 0.0f)]
        public float Friction
        {
            get { return def.Friction; }
            set
            {
                if (def.Friction != value)
                {
                    def.Friction = value;
                    if (null != body) body.Friction = value;
                    Change("Friction");
                }
            }
        }

        [PluginProperty("Linear Damping", "", 0.0f)]
        public float LinearDamping
        {
            get { return def.LinearDamping; }
            set
            {
                if (def.LinearDamping != value)
                {
                    def.LinearDamping = value;
                    if (null != body) body.LinearDamping = value;
                    Change("LinearDamping");
                }
            }
        }

        [PluginProperty("X Velocity", "", 0.0f)]
        public float XVelocity
        {
            get
            {
                float velocity = def.XVelocity;
                if (null != body) velocity = body.LinearVelocity.X;
                return PhysicsManager.ConvertVelocityToGame(velocity);
            }
            set 
            {
                value = PhysicsManager.ConvertVelocityToFarseer(value);
                if (null == body || value != body.LinearVelocity.X)
                {
                    def.XVelocity = value;
                    if (null != body) body.LinearVelocity = new Vector2(value, body.LinearVelocity.Y);
                    Change("XVelocity");
                }
            }
        }

        [PluginProperty("Y Velocity", "", 0.0f)]
        public float YVelocity
        {
            get
            {
                float velocity = def.YVelocity;
                if (null != body) velocity = body.LinearVelocity.Y;
                return PhysicsManager.ConvertVelocityToGame(velocity);
            }
            set
            {
                value = PhysicsManager.ConvertVelocityToFarseer(value);
                if (null == body || value != body.LinearVelocity.Y)
                {
                    def.YVelocity = value;
                    if (null != body) body.LinearVelocity = new Vector2(body.LinearVelocity.X, value);
                    Change("YVelocity");
                }
            }
        }

        [PluginProperty("Speed", "Automatically set X and Y velocity based on desired speed", 0.0f)]
        public float Speed
        {
            get
            {
                float speed = def.Speed;
                if (null != body)
                {
                    speed = (float)Math.Sqrt(Math.Pow((double)body.LinearVelocity.X, 2) + Math.Pow((double)body.LinearVelocity.Y, 2));
                }
                return PhysicsManager.ConvertVelocityToGame(speed);
            }
            set
            {
                value = PhysicsManager.ConvertVelocityToFarseer(value);
                def.Speed = value;
                if (null != body)
                {
                    float velocityX = (float)Math.Cos(PhysicsManager.DegreesToRadians(transform.Rotation)) * value;
                    float velocityY = (float)Math.Sin(PhysicsManager.DegreesToRadians(transform.Rotation)) * value;

                    if (transform.Rotation > 180.0f)
                    {
                        velocityY = -velocityY;
                    }

                    body.LinearVelocity = new Vector2(velocityX, velocityY);
                }
            }
        }

        [PluginProperty("Angular Velocity", "", 0.0f)]
        public float AngularVelocity
        {
            get { return null != body ? body.AngularVelocity : def.AngularVelocity; }
            set
            {
                if (null == body || value != body.AngularVelocity)
                {
                    def.AngularVelocity = value;
                    if (null != body) body.AngularVelocity = value;
                    Change("AngularVelocity");
                }
            }
        }

        [PluginProperty("Body Type", "Object moves when hit", Kinectitude.Core.ComponentInterfaces.BodyType.Dynamic)]
        public Kinectitude.Core.ComponentInterfaces.BodyType BodyType
        {
            get { return def.BodyType; }
            set
            {
                if (value != def.BodyType)
                {
                    def.BodyType = value;
                    if (null != body) body.BodyType = ConvertBodyTypeToFarseer(value);
                    Change("BodyType");
                }
            }
 
        }

        [PluginProperty("Fixed Rotation", "Object can rotate as it moves", false)]
        public bool FixedRotation
        {
            get { return def.FixedRotation; }
            set
            {
                if (value != def.FixedRotation)
                {
                    def.FixedRotation = value;
                    if (null != body) body.FixedRotation = value;
                    Change("FixedRotation");
                }
            }
        }

        [PluginProperty("Ignores Collisions", "Any entities that match this type matcher will pass through this object")]
        public TypeMatcher IgnoreCollisionsWith
        {
            get { return def.IgnoreCollisionsWith; }
            set
            {
                if (def.IgnoreCollisionsWith != value)
                {
                    //TODO override = operator for TypeMatcher
                    def.IgnoreCollisionsWith = value;
                    Change("IgnoreCollisionsWith");
                }
            }
        }

        private void CheckCrossesLine()
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
                        prev = transform.PreviousX;
                        next = transform.X;
                    }
                    else
                    {
                        prev = transform.PreviousY;
                        next = transform.Y;
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

        public override void OnUpdate(float t)
        {
            if (body.Awake)
            {
                CheckCrossesLine();
            }
            
            transform.Update();
        }

        public void AddCrossLineEvent(CrossesLineEvent evt)
        {
            crossesLineEvents.Add(evt);
        }

        public void AddCollisionEvent(CollisionEvent evt)
        {
            collisionEvents.Add(evt);
        }

        public override void Ready()
        {
            manager = GetManager<PhysicsManager>();
            manager.Add(this);

            var transformComponent = GetComponent<TransformComponent>();
            transform = new PhysicsTransform(transformComponent.Transform);
            transformComponent.Transform = transform;

            CreateBody();
            transform.Body = body;
        }

        private void CreateBody()
        {
            if (ShapeType.Ellipse == Shape)
            {
                float xRadius = PhysicsManager.ConvertDistanceToFarseer(transform.Width / 2.0f);
                float yRadius = PhysicsManager.ConvertDistanceToFarseer(transform.Height / 2.0f);

                if (xRadius != yRadius)
                {
                    body = BodyFactory.CreateEllipse(manager.PhysicsWorld, xRadius, yRadius, 12, 1f);
                }
                else
                {
                    body = BodyFactory.CreateCircle(manager.PhysicsWorld, xRadius, 1f);
                }
            }
            else
            {
                float width = PhysicsManager.ConvertDistanceToFarseer(transform.Width);
                float height = PhysicsManager.ConvertDistanceToFarseer(transform.Height);
                body = BodyFactory.CreateRectangle(manager.PhysicsWorld, width, height, 1f);
            }

            body.BodyType = ConvertBodyTypeToFarseer(def.BodyType);
            body.FixedRotation = def.FixedRotation;
            body.AngularVelocity = def.AngularVelocity;
            body.Restitution = def.Restitution;
            body.Friction = def.Friction;
            body.LinearDamping = def.LinearDamping;
            body.LinearVelocity = new Vector2(def.XVelocity, def.YVelocity);
            body.UserData = this;

            //Add a listener for collisions
            body.OnCollision += OnCollision;
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

            if (IgnoreCollisionsWith != null && IgnoreCollisionsWith.MatchAndSet(collidedWith)) return false;

            //Allow the collison to occur
            return true;
        }

        public override void Destroy()
        {
            manager.Remove(this);
            manager.PhysicsWorld.RemoveBody(body);
        }
    }
}
