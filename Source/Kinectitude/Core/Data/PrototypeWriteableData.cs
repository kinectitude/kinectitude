using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core
{
    public class PrototypeWriteableData : WriteableData
    {
        private readonly List<HashSet<int>> prototypes;
        
        internal PrototypeWriteableData(List<HashSet<int>> prototypes)
        {
            this.prototypes = prototypes;
        }

        public override bool IfMatchSet(DataContainer dataContainer)
        {
            if (dataContainer is DataContainer) return false;
            if (dataContainer is Scene) return false;
            int id = (dataContainer as Entity).Id;
            foreach (HashSet<int> prototype in prototypes)
            {
                if (prototype.Contains(id))
                {
                    DataContainer = dataContainer;
                    return true;
                }
            }
            return false;
        }
    }
}
