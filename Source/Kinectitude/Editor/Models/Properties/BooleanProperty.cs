using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public class BooleanProperty : Property<bool>
    {
        public override string StringValue
        {
            get { return Value.ToString(); }
        }

        public BooleanProperty(PropertyDescriptor descriptor) : base(descriptor) { }

        public override bool TryParse(string input)
        {
            bool parsed = false;
            bool ret = bool.TryParse(input, out parsed);
            Value = parsed;
            return ret;
        }

        public override string ToString()
        {
            return string.Format("Key: {0}, Type: Boolean, Value: {1}", Descriptor.Key, Value);
        }
    }
}
