using System.Collections.Generic;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Data
{
    internal class MultiTypeMatcher : TypeMatcher
    {
        private readonly List<ITypeMatcher> readables;

        internal MultiTypeMatcher(List<ITypeMatcher> readables)
        {
            this.readables = readables;
        }

        public override bool MatchAndSet(IDataContainer DataContainer)
        {
            foreach (TypeMatcher r in readables)
            {
                if (MatchAndSet(DataContainer))
                {
                    DataContainer = DataContainer as DataContainer;
                    return true;
                }
            }
            return false;
        }
    }
}
