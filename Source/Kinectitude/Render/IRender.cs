//-----------------------------------------------------------------------
// <copyright file="IRender.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Base;
using SlimDX.Direct2D;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Components;
using Kinectitude.Core.ComponentInterfaces;

namespace Kinectitude.Render
{
    [Requires(typeof(TransformComponent))]
    public interface IRender : IUpdateable
    {
        bool FixedPosition { get; }

        void Render(RenderTarget renderTarget);
    }
}
