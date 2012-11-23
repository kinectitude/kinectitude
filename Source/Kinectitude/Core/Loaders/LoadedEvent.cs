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
