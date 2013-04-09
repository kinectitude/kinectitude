//-----------------------------------------------------------------------
// <copyright file="LoadedEvent.cs" company="Kinectitude">
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

namespace Kinectitude.Core.Loaders
{
    internal sealed class LoadedEvent : LoadedObject
    {
        private readonly string type;
        private readonly List<LoadedBaseAction> actions = new List<LoadedBaseAction>();

        internal LoadedEvent(string type, PropertyHolder values, LoadedEntity entity, LoaderUtility loaderUtil)
            : base(values, loaderUtil)
        {
            this.type = type;
            entity.AddLoadedEvent(this);
        }


        internal Event Create(Entity entity)
        {
            Event evt = ClassFactory.Create<Event>(type);
            evt.Entity = entity;
            setValues(evt, evt, entity, entity.Scene);
            foreach (LoadedBaseAction action in actions)
            {
                evt.AddAction(action.Create(evt));
            }
            return evt;
        }

        internal void AddAction(LoadedBaseAction action)
        {
            actions.Add(action);
        }

    }
}
