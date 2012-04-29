using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public sealed class BooleanAttribute : Attribute<bool>
    {
        public override string StringValue
        {
            get { return Value.ToString(); }
        }

        public BooleanAttribute(string key) : base(key) { }

        public override string ToString()
        {
            return string.Format("Key: {0}, Type: Boolean, Value: '{1}'", Key, Value);
        }
    }
}
