using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class SingleTypeMatcher : TypeMatcher
    {
        internal SingleTypeMatcher(DataContainer dataContainer)
        {
            DataContainer = dataContainer;
        }

        public override bool MatchAndSet(IDataContainer dataContainer)
        {
            //no need to set, it already is
            return dataContainer == DataContainer;
        }
    }
}
