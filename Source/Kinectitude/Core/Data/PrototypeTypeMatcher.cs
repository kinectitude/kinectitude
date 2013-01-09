using System;
using System.Collections.Generic;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class PrototypeTypeMatcher : MultiTypeMatcher
    {
        private readonly HashSet<int> prototype;

        internal PrototypeTypeMatcher(HashSet<int> prototype)
        {
            this.prototype = prototype;
        }

        public override bool MatchAndSet(IEntity entity)
        {
            Entity who = entity as Entity;
            if (null == who) return false;
            if (prototype.Contains(who.Id))
            {
                OldEntity = this.Entity;
                Entity = who;
                foreach (Action<DataContainer> toNotify in notify)
                {
                    toNotify(Entity);
                }
                return true;
            }
            return false;
        }
    }
}
