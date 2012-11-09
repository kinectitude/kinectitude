using System;
using System.Collections.Generic;
using System.Linq;
using Kinectitude.Core.Base;
using Kinectitude.Core.Exceptions;
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
                if (Entity == null) return ConstantReader.NullValue;
                return Entity[key];
            }
        }

        internal ValueReader getComponentValue(string comopnentName, string param)
        {
            if (Entity == null) return ConstantReader.NullValue;
            Component component = Entity.GetComponent(comopnentName);
            if (component == null) return ConstantReader.NullValue;
            return ParameterValueReader.getParameterValueReader(component, param, Entity.Scene);
        }

        internal abstract void NotifyOfChange(Action<DataContainer> action);

        public string NameOfLastMatch
        {
            get 
            {
                if (null == Entity)
                {
                    return null;
                }
                return Entity.Name; 
            }
        }

        public int IdOfLastMatch { get { return Entity.Id; } }
    }
}
