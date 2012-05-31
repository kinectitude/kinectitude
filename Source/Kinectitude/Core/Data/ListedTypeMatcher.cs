using System;
using System.Collections.Generic;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal class ListedTypeMatcher : MultiTypeMatcher
    {
        private readonly List<ITypeMatcher> readables;
        
        internal ListedTypeMatcher(List<ITypeMatcher> readables)
        {
            this.readables = readables;
        }

        public override bool MatchAndSet(IDataContainer DataContainer)
        {
            foreach (TypeMatcher r in readables)
            {
                if (MatchAndSet(DataContainer))
                {
                    OldDataContainer = this.DataContainer;
                    this.DataContainer = DataContainer as DataContainer;
                    foreach (Action<DataContainer> toNotify in notify)
                    {
                        toNotify(this.DataContainer);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
