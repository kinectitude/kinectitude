using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public sealed class IntegerAttribute : Attribute<int>
    {
        public override string StringValue
        {
            get { return Value.ToString(); }
        }

        public IntegerAttribute(string key) : base(key) { }

        public override string ToString()
        {
            return string.Format("Key: {0}, Type: Integer, Value: '{1}'", Key, Value);
        }
    }
}
