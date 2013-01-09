using System;
using System.Collections.Generic;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class ListedTypeMatcher : MultiTypeMatcher
    {
        private readonly List<TypeMatcher> Readables;

        internal ListedTypeMatcher(List<TypeMatcher> readables) { Readables = readables; }

        public override bool MatchAndSet(IEntity entity)
        {
            Entity who = entity as Entity;
            if (null == who) return false;
            foreach (TypeMatcher r in Readables)
            {
                
                if (r.MatchAndSet(entity))
                {
                    OldEntity = Entity;
                    Entity = who;
                    foreach (Action<DataContainer> toNotify in notify) toNotify(this.Entity);
                    return true;
                }
            }
            return false;
        }
    }
}
