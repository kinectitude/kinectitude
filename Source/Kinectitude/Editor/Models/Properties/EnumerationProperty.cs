using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public class EnumerationProperty : Property<string>
    {
        public new string Value
        {
            get { return base.Value; }
            set
            {
                string found = Descriptor.Enumeration.FirstOrDefault(x => x == value);
                if (null != found)
                {
                    base.Value = found;
                }
            }
        }

        public override string StringValue
        {
            get { return null != Value ? Value : string.Empty; }
        }

        public EnumerationProperty(PropertyDescriptor descriptor) : base(descriptor) { }

        public override bool TryParse(string input)
        {
            bool ret = false;
            string found = Descriptor.Enumeration.FirstOrDefault(x => x == input);
            if (null != found)
            {
                Value = found;
                ret = true;
            }
            return ret;
        }

        public override string ToString()
        {
            return string.Format("Key: {0}, Type: Enumeration, Value: {1}", Descriptor.Key, Value);
        }
    }
}
