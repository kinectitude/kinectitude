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
            foreach (TypeMatcher r in Readables)
            {
                if (r.MatchAndSet(entity))
                {
                    OldEntity = Entity;
                    Entity = entity as Entity;
                    foreach (Action<DataContainer> toNotify in notify) toNotify(this.Entity);
                    return true;
                }
            }
            return false;
        }
    }
}
