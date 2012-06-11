using System;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class SingleTypeMatcher : TypeMatcher
    {
        internal SingleTypeMatcher(DataContainer dataContainer)
        {
            DataContainer = dataContainer;
        }

        public override bool MatchAndSet(IEntity dataContainer)
        {
            //no need to set, it already is
            return dataContainer == DataContainer;
        }

        internal override void NotifyOfChange(Action<DataContainer> action) { }
    }
}
