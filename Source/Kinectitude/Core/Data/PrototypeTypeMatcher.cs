using System.Collections.Generic;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class PrototypeTypeMatcher : TypeMatcher
    {
        private readonly HashSet<int> prototype;

        internal PrototypeTypeMatcher(HashSet<int> prototype)
        {
            this.prototype = prototype;
        }

        public override bool MatchAndSet(IDataContainer dataContainer)
        {
            if (prototype.Contains(dataContainer.Id))
            {
                DataContainer = dataContainer as DataContainer;
                return true;
            }
            return false;
        }
    }
}
