using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core
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
