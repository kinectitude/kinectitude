//-----------------------------------------------------------------------
// <copyright file="DefaultEntityVisual.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models;

namespace Kinectitude.Editor.Views.Scenes.Presenters
{
    internal sealed class DefaultEntityVisual : EntityVisual
    {
        public DefaultEntityVisual(EntityPresenter presenter, Component render, Entity entity) : base(presenter, render, entity) { }
    }
}
