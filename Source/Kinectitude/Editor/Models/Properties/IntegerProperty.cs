using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public class IntegerProperty : Property<int>
    {
        public override string StringValue
        {
            get { return Value.ToString(); }
        }

        public IntegerProperty(PropertyDescriptor descriptor) : base(descriptor) { }

        public override bool TryParse(string input)
        {
            int parsed = 0;
            bool ret = int.TryParse(input, out parsed);
            Value = parsed;
            return ret;
        }

        public override string ToString()
        {
            return string.Format("Key: {0}, Type: Integer, Value: {1}", Descriptor.Key, Value);
        }
    }
}
