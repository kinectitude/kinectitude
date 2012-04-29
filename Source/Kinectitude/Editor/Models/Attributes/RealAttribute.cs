using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public sealed class RealAttribute : Attribute<double>
    {
        public override string StringValue
        {
            get { return Value.ToString(); }
        }

        public RealAttribute(string key) : base(key) { }

        public override string ToString()
        {
            return string.Format("Key: {0}, Type: Real, Value: '{1}'", Key, Value);
        }
    }
}
