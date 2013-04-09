//-----------------------------------------------------------------------
// <copyright file="PopSceneAction.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Actions
{
    [Plugin("pop this scene", "Pop scene")]
    internal sealed class PopSceneAction : Action
    {
        public PopSceneAction() { }

        public override void Run()
        {
            Game game = Event.Entity.Scene.Game;
            game.PopScene();
        }
    }
}
