﻿using Kinectitude.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Core.ComponentInterfaces;
using System.Collections.Generic;
using System;
//using Kinectitude.Core.Exceptions;

namespace Kinectitude.Physics
{
    [Plugin("Fire an event when a collision between bodies occurs", "")]
    public class CollisionEvent : Event
    {
        [Plugin("Collides with","")]
        public ITypeMatcher CollidesWith { get; set; }

        public CollisionEvent() { }

        public override void OnInitialize()
        {
            PhysicsComponent pc = GetComponent<APhysicsComponent>() as PhysicsComponent;
            if (null == pc)
            {
                List<Type> missing = new List<Type>();
                missing.Add(typeof(PhysicsComponent));
                //TODO this will be done automatically
                //throw MissingRequirementsException.MissingRequirement(this, missing);
            }
            pc.AddCollisionEvent(this);
        }
    }
}