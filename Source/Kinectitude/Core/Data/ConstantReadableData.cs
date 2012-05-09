using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal class ConstantReadableData : ReadableData
    {
        internal ConstantReadableData(DataContainer dataContainer)
        {
            DataContainer = dataContainer;
        }

        public override bool MatchAndSet(DataContainer dataContainer)
        {
            //no need to set, it already is
            return dataContainer == DataContainer;
        }
    }
}
