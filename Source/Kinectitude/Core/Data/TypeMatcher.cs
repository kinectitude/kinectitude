using System.Collections.Generic;
using System.Linq;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Core.Exceptions;

namespace Kinectitude.Core.Data
{
    internal abstract class TypeMatcher : ITypeMatcher
    {

        internal DataContainer DataContainer { get; set; }

        public abstract bool MatchAndSet(IDataContainer entity);

        internal static TypeMatcher CreationHelper(string value, Dictionary<string,HashSet<int>> from)
        {
            value = value.Substring(1);
            HashSet<int> matcher = from[value];
            if (null == matcher)
            {
                throw new NoSuchPrototypeException(value);
            }
            return new PrototypeTypeMatcher(matcher);
        }

        internal static TypeMatcher CreateTypeMatcher(string value, Event evt, Entity entity)
        {
            Scene scene = entity.Scene;
            if (value.Contains(' '))
            {
                List<ITypeMatcher> readableList = new List<ITypeMatcher>();
                string[] readables = value.Split(' ');
                foreach (string readable in readables)
                {
                    readableList.Add(CreateTypeMatcher(readable, evt, entity));
                }
                return new MultiTypeMatcher(readableList);
            }
            if ('!' == value[0])
            {
                if (null == evt)
                {
                    throw new IllegalPlacementException("!", "events or actions");
                }
                value = value.Substring(1);
                TypeMatcher matcher = evt.AvailableSelectors[value];
                if (null == matcher)
                {
                    throw new InvalidAttributeException
                        (value, null == entity.Name ? entity.Id.ToString() : entity.Name);
                }
                return matcher;
            }
            if ('$' == value[0])
            {
                CreationHelper(value, scene.IsType);
            }
            if ('#' == value[0])
            {
                CreationHelper(value, scene.IsExactType);
            }
            if ("game" == value)
            {
                return new SingleTypeMatcher(entity.Scene.Game);
            }
            if ("scene" == value)
            {
                return new SingleTypeMatcher(entity.Scene);
            }
            if ("this" == value)
            {
                return new SingleTypeMatcher(entity);
            }
            entity = scene.EntityByName(value);
            return new SingleTypeMatcher(entity);
        }

        internal string this[string key]
        {
            get
            {
                return DataContainer[key];
            }
        }
    }
}
