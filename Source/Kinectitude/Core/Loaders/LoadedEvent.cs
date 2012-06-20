using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Loaders
{
    internal class LoadedEvent : LoadedObject
    {
        private readonly string type;
        private readonly List<LoadedBaseAction> actions = new List<LoadedBaseAction>();

        internal LoadedEvent(string type, List<Tuple<string, string>> values)
            : base(values)
        {
            this.type = type;
        }


        internal Event Create(Entity entity)
        {
            Event evt = ClassFactory.Create<Event>(type);
            evt.Entity = entity;
            setValues(evt, evt);
            foreach (LoadedBaseAction action in actions)
            {
                evt.AddAction(action.Create(evt));
            }
            return evt;
        }

    }
}
