using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core
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
