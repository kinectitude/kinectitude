using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core
{
    public class SpecificWriter
    {
        private readonly WriteableData writeableData;
        private readonly string key;

        internal SpecificWriter(WriteableData writeableData, string key)
        {
            this.writeableData = writeableData;
            this.key = key;
        }

        public void SetValue(string value)
        {
            writeableData[key] = value;
        }
        
        public string GetValue()
        {
            return writeableData[key];
        }
    }
}
