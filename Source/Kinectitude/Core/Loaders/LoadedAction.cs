//-----------------------------------------------------------------------
// <copyright file="LoadedAction.cs" company="Kinectitude">
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
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Loaders
{
    internal sealed class LoadedAction : LoadedBaseAction
    {
        private readonly string type;

        internal LoadedAction(string type, PropertyHolder values, LoaderUtility loaderUtil) : base(values, loaderUtil)
        {
            this.type = type;
        }

        internal override Action Create(Event evt)
        {
            Action action = ClassFactory.Create<Action>(type);
            setValues(action, evt, evt.Entity, evt.Entity.Scene);
            action.Event = evt;
            return action;
        }
    }
}
