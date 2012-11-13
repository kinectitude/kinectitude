using System;
using System.Collections.Generic;
using System.Linq;
using Kinectitude.Core.Base;
using Kinectitude.Core.Loaders;

namespace Kinectitude.Core.Data
{
    public abstract class TypeMatcher
    {
        internal Entity Entity { get; set; }

        //used to see if an expression has changed when the data container changes
        internal Entity OldEntity { get; set; }

        public abstract bool MatchAndSet(IEntity entity);

        internal ValueReader this[string key]
        {
            get
            {
                if (Entity == null) Game.CurrentGame.Die("Can't read from an unmatched entity");
                return Entity[key];
            }
            set
            {
                if (Entity == null) Game.CurrentGame.Die("Can't write from an unmatched entity");
                Entity[key] = value;
            }
        }

        internal ValueReader getComponentValue(string comopnentName, string param)
        {
            if (Entity == null) Game.CurrentGame.Die("Can't read from an unmatched entity");
            Component component = Entity.GetComponent(comopnentName);
            return ParameterValueReader.getParameterValueReader(component, param, Entity.Scene);
        }

        internal void setComponentValue(string comopnentName, string param, ValueReader value)
        {
            if (Entity == null) Game.CurrentGame.Die("Can't write from an unmatched entity");
            Component component = Entity.GetComponent(comopnentName);
            ParameterValueReader.getParameterValueReader(component, param, Entity.Scene).GetValueWriter().SetValue(value);
        }

        internal abstract void NotifyOfChange(Action<DataContainer> action);
    }
}