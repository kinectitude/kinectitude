using System;
using System.Collections.Generic;
using System.Linq;
using Kinectitude.Core.Base;
using Kinectitude.Core.Exceptions;
using Kinectitude.Core.Loaders;

namespace Kinectitude.Core.Data
{
    public abstract class TypeMatcher : IAssignable
    {
        internal DataContainer DataContainer { get; set; }

        //used to see if an expression has changed when the data container changes
        internal DataContainer OldDataContainer { get; set; }

        public abstract bool MatchAndSet(IEntity entity);

        internal static TypeMatcher CreationHelper(string value, Dictionary<string,HashSet<int>> from, GameLoader loader)
        {
            value = value.Substring(1);
            HashSet<int> matcher = null;
            from.TryGetValue(value, out matcher);
            if (null == matcher)
            {
                matcher = new HashSet<int>();
                from[value] = matcher;
            }
            return new PrototypeTypeMatcher(matcher);
        }

        internal ValueReader this[string key]
        {
            get
            {
                if (DataContainer.Deleted)
                {
                    return null;
                }
                return DataContainer[key];
            }
        }

        internal abstract void NotifyOfChange(Action<DataContainer> action);


        public string NameOfLastMatch
        {
            get 
            {
                if (null == DataContainer)
                {
                    return null;
                }
                return DataContainer.Name; 
            }
        }


        public int IdOfLastMatch { get { return DataContainer.Id; } }

        public void SetParam(object obj, string param) { ClassFactory.SetParam<TypeMatcher>(obj, param, this); }
    }
}
