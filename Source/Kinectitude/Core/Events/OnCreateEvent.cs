//-----------------------------------------------------------------------
// <copyright file="OnCreateEvent.cs" company="Kinectitude">
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
using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;

namespace Kinectitude.Core.Events
{
    [Plugin("when this entity is created", "Entity created")]
    class OnCreateEvent : Event
    {
        public override void OnInitialize()
        {
            Entity.addOnCreate(this);
        }
    }
}
