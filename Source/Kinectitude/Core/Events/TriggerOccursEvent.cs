//-----------------------------------------------------------------------
// <copyright file="TriggerOccursEvent.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Events
{
    // Trigger can just be an event that does nothing special on its own, but can be treated special

    [Plugin("when trigger {Trigger} occurs", "Trigger occurs")]
    internal sealed class TriggerOccursEvent : Event
    {

        public TriggerOccursEvent() { }

        [PluginProperty("Trigger", "")]
        public string Trigger { get; set; }

        public override void OnInitialize()
        {
            Scene scene = Entity.Scene;
            scene.RegisterTrigger(Trigger, this);
        }
    }
}
