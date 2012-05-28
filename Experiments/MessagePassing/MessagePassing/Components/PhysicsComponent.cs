using System;
using System.Collections.Generic;
using MessagePassing.Public;

namespace MessagePassing.Components
{
    public class PhysicsComponent : Component
    {
        private string type;
        private double velocityX;
        private double velocityY;
        private PhysicsManager manager;
        private readonly Dictionary<string, MessageCallback> collisionCallbacks;

        public string Type
        {
            get { return type; }
            set
            {
                // Any component that wants to notify other of property changes can use the EventManager.
                // Anything published from here gets prefixed with "Physics." for security.

                if (type != value)
                {
                    type = value;
                    Messenger.Publish("Type", type);
                }
            }
        }

        public double LinearVelocityX
        {
            get { return velocityX; }
            set
            {
                if (velocityX != value)
                {
                    velocityX = value;
                    Messenger.Publish("LinearVelocityX", velocityX);
                }
            }
        }

        public double LinearVelocityY
        {
            get { return velocityY; }
            set
            {
                if (velocityY != value)
                {
                    velocityY = value;
                    Messenger.Publish("LinearVelocityY", velocityY);
                }
            }
        }

        public PhysicsComponent()
        {
            collisionCallbacks = new Dictionary<string, MessageCallback>();
        }

        public override void Initialize()
        {
            manager = ComponentContainer.GetManager<PhysicsManager>();
            manager.Add(this);
        }

        public override void Update(double frameDelta) { }

        /// <summary>
        /// Method called by the PhysicsManager to tell this component that it has collided with another component.
        /// </summary>
        /// <param name="other">The physics component this one collided with.</param>
        public void Collide(PhysicsComponent other)
        {
            MessageCallback callback;
            collisionCallbacks.TryGetValue(other.Type, out callback);

            if (null != callback)
            {
                callback(null);
            }
        }

        /// <summary>
        /// Handles requests to subscribe to the Collision event. Takes a single strongly-typed argument which is the type of entity to check
        /// for collision with. 
        /// </summary>
        /// <param name="callback">The callback to run when this event occurs</param>
        /// <param name="with">The type of entity the caller wants to check for collision with</param>
        [Event]
        public void SubscribeCollision(MessageCallback callback, string with)
        {
            MessageCallback registeredCallback;
            collisionCallbacks.TryGetValue(with, out registeredCallback);

            if (null == registeredCallback)
            {
                registeredCallback = callback;
            }
            else
            {
                registeredCallback += callback;
            }

            collisionCallbacks[with] = registeredCallback;
        }

        /// <summary>
        /// Any published "Physics.SetType" message will be handled by this method.
        /// </summary>
        /// <param name="type">The type to set</param>
        [Action]
        public void SetType(string type)
        {
            Type = type;    // This will in turn publish the "Physics.Type" message which others can pick up if they want
        }

        /// <summary>
        /// Any published "SetLinearVelocity" message will be handled by this method.
        /// </summary>
        /// <param name="dx">The new horizontal velocity</param>
        /// <param name="dy">The new vertical velocity</param>
        [Action]
        public void SetLinearVelocity(double dx, double dy)
        {
            LinearVelocityX = dx;
            LinearVelocityY = dy;
        }
    }
}
