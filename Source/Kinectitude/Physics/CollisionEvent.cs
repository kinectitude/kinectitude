using Kinectitude.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Core.Interfaces;
using System.Collections.Generic;
using System;
using Kinectitude.Core.Exceptions;

namespace Kinectitude.Physics
{
    [Plugin("Fire an event when a collision between bodies occurs", "")]
    public class CollisionEvent : Event
    {
        [Plugin("Collides with","")]
        public ReadableData CollidesWith { get; set; }

        public CollisionEvent() { }

        public override void OnInitialize()
        {
            PhysicsComponent pc = Entity.GetComponent(typeof(IPhysicsComponent)) as PhysicsComponent;
            if (null == pc)
            {
                List<Type> missing = new List<Type>();
                missing.Add(typeof(PhysicsComponent));
                throw MissingRequirementsException.MissingRequirement(this, missing);
            }
            pc.AddCollisionEvent(this);
        }
    }
}
