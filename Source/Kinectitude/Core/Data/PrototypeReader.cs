using System.Collections.Generic;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    public class PrototypeReader : ReadableData
    {
        private readonly HashSet<int> prototype;

        internal PrototypeReader(HashSet<int> prototype)
        {
            this.prototype = prototype;
        }

        public override bool MatchAndSet(DataContainer dataContainer)
        {
            if (prototype.Contains(dataContainer.Id))
            {
                DataContainer = dataContainer;
                return true;
            }
            return false;
        }
    }
}
