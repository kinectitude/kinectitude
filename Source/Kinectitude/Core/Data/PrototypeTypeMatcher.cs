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

        public override bool MatchAndSet(IEntity dataContainer)
        {
            if (prototype.Contains(dataContainer.Id))
            {
                OldDataContainer = this.DataContainer;
                this.DataContainer = dataContainer as DataContainer;
                foreach (Action<DataContainer> toNotify in notify)
                {
                    toNotify(this.DataContainer);
                }
                return true;
            }
            return false;
        }
    }
}
