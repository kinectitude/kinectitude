using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public class TextProperty : Property<string>
    {
        public override string StringValue
        {
            get { return null != Value ? Value : string.Empty; }
        }

        public TextProperty(PropertyDescriptor descriptor) : base(descriptor) { }

        public override bool TryParse(string input)
        {
            Value = null != input ? input : string.Empty;
            return true;
        }

        public override string ToString()
        {
            return string.Format("Key: {0}, Type: Text, Value: {1}", Descriptor.Key, Value);
        }
    }
}
