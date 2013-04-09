//-----------------------------------------------------------------------
// <copyright file="FireTriggerAction.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    [Plugin("fire trigger {Name}", "Fire a trigger")]
    internal sealed class FireTriggerAction : Action
    {
        [PluginProperty("Trigger", "")]
        public ValueReader Name { get; set; }

        public FireTriggerAction() { }

        public override void Run()
        {
            Event.Entity.Scene.FireTrigger(Name);
        }
    }
}
