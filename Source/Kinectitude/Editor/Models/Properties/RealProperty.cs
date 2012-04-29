using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public class RealProperty : Property<double>
    {
        public override string StringValue
        {
            get { return Value.ToString(); }
        }

        public RealProperty(PropertyDescriptor descriptor) : base(descriptor) { }

        public override bool TryParse(string input)
        {
            double parsed = 0;
            bool ret = double.TryParse(input, out parsed);
            Value = parsed;
            return ret;
        }

        public override string ToString()
        {
            return string.Format("Key {0}, Type: Real, Value: {1}", Descriptor.Key, Value);
        }
    }
}
