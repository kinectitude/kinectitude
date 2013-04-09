//-----------------------------------------------------------------------
// <copyright file="CreateEntityAction.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Attributes;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Actions
{
    [Plugin("create entity based on {Prototype}", "Create an entity")]
    class CreateEntityAction : Action
    {
        [PluginProperty("Prototype", "Name of Prototype to make")]
        public string Prototype { get; set; }

        public override void Run()
        {
            Event.Entity.Scene.CreateEntity(Prototype);
        }
    }
}
