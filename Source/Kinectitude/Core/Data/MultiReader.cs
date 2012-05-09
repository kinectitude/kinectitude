using System.Collections.Generic;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    public class MultiReader : ReadableData
    {
        private readonly List<ReadableData> readables;

        internal MultiReader(List<ReadableData> readables)
        {
            this.readables = readables;
        }

        public override bool MatchAndSet(DataContainer dataContainer)
        {
            foreach (ReadableData r in readables)
            {
                if (MatchAndSet(dataContainer))
                {
                    DataContainer = dataContainer;
                    return true;
                }
            }
            return false;
        }
    }
}
