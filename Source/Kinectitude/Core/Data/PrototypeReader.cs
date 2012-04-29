﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core
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
