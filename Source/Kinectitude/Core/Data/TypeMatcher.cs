using System;
using System.Collections.Generic;
using System.Linq;
using Kinectitude.Core.Base;
using Kinectitude.Core.Exceptions;
using Kinectitude.Core.Loaders;

namespace Kinectitude.Core.Data
{
    internal abstract class TypeMatcher : ITypeMatcher
    {
        internal const char TypeChar = '$';
        internal const char ExactTypeChar = '#';
        internal const char ParentChar = '@';

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
                if (!loader.AvaliblePrototypes.Contains(value))
                {
                    throw new NoSuchPrototypeException(value);
                }
                else
                {
                    matcher = new HashSet<int>();
                    from[value] = matcher;
                }
            }
            return new PrototypeTypeMatcher(matcher);
        }

        internal static TypeMatcher CreateTypeMatcher(string value, Event evt, Entity entity)
        {
            Scene scene = null == entity ? null : entity.Scene;
            if (value.Contains(' '))
            {
                List<ITypeMatcher> readableList = new List<ITypeMatcher>();
                string[] readables = value.Split(' ');
                foreach (string readable in readables)
                {
                    readableList.Add(CreateTypeMatcher(readable, evt, entity));
                }
                return new ListedTypeMatcher(readableList);
            }
            if (ParentChar == value[0])
            {
                if (null == evt)
                {
                    throw new IllegalPlacementException(ParentChar.ToString(), "events or actions");
                }
                value = value.Substring(1);
                TypeMatcher matcher = ClassFactory.GetParam<ITypeMatcher>(evt, value) as TypeMatcher;
                if (null == matcher)
                {
                    throw new InvalidAttributeException
                        (value, null == entity.Name ? entity.Id.ToString() : entity.Name);
                }
                return matcher;
            }
            if (TypeChar == value[0])
            {
                return CreationHelper(value, scene.IsType, scene.Game.GameLoader);
            }
            if (ExactTypeChar == value[0])
            {
                return CreationHelper(value, scene.IsExactType, scene.Game.GameLoader);
            }
            if ("game" == value)
            {
                return new SingleTypeMatcher(Game.CurrentGame);
            }
            if ("scene" == value)
            {
                return new SingleTypeMatcher(entity.Scene);
            }
            if ("this" == value)
            {
                return new SingleTypeMatcher(entity);
            }
            entity = scene.GetEntity(value);
            return new SingleTypeMatcher(entity);
        }

        internal string this[string key]
        {
            get
            {
                if (DataContainer.Deleted)
                {
                    return "";
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


        public int IdOfLastMatch
        {
            get { return DataContainer.Id; }
        }
    }
}
