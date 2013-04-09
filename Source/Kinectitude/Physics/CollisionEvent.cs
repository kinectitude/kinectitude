//-----------------------------------------------------------------------
// <copyright file="CollisionEvent.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Physics
{
    [Plugin("when this entity collides with {CollidesWith}", "Entity collides")]
    public class CollisionEvent : Event
    {
        [PluginProperty("Collides With","")]
        public TypeMatcher CollidesWith { get; set; }

        public CollisionEvent() { }

        public override void OnInitialize()
        {
            PhysicsComponent pc = GetComponent<PhysicsComponent>();
            pc.AddCollisionEvent(this);
        }
    }
}
