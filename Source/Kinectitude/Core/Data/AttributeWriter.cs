using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class AttributeWriter : ValueWriter
    {
        private readonly string key;
        internal AttributeWriter(string key, DataContainer dataContainer) : base(dataContainer) 
        {
            this.key = key;
        }

        public override string Value
        {
            get { return DataContainer[key]; }
            set { DataContainer[key] = value; }
        }
    }
}
