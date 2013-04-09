//-----------------------------------------------------------------------
// <copyright file="SceneStartsEvent.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Events
{
    [Plugin("when the scene starts", "Scene starts")]
    internal sealed class SceneStartsEvent : Event
    {
        public SceneStartsEvent() { }

        public override void OnInitialize()
        {
            Scene scene = Entity.Scene;
            scene.OnStart.Add(this);
        }
    }
}
