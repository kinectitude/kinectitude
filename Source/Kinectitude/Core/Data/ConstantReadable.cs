using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core
{
    internal class ConstantReadable : SpecificReadable
    {
        private readonly string value;

        internal ConstantReadable(string value)
        {
            this.value = value;
        }
        
        public override string GetValue()
        {
            return value;
        }
    }
}
