//-----------------------------------------------------------------------
// <copyright file="MouseFollowComponent.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.AbstractComponents;
using Kinectitude.Core.Attributes;

namespace Kinectitude.Input
{
    [Plugin("Mouse Motion Component", "")]
    [Provides(typeof(BasicFollowComponent))]
    public class MouseFollowComponent : BasicFollowComponent
    {
        private MouseManager mouseManager;

        public override void Ready()
        {
            mouseManager = GetManager<MouseManager>();
            mouseManager.Add(this);
            base.Ready();
        }

        public override void Destroy()
        {
            mouseManager.Remove(this);
        }
    }
}
