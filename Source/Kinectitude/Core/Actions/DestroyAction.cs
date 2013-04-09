//-----------------------------------------------------------------------
// <copyright file="DestroyAction.cs" company="Kinectitude">
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
    [Plugin("destroy this entity", "Destroy entity")]
    class DestroyAction : Action
    {
        public override void Run()
        {
            Event.Entity.Destroy();
        }
    }
}
