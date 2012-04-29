using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public sealed class TextAttribute : Attribute<string>
    {
        public override string StringValue
        {
            get { return null != Value ? Value : string.Empty; }
        }

        public TextAttribute(string key) : base(key) { }

        public override string ToString()
        {
            return string.Format("Key: {0}, Type: Text, Value: '{1}'", Key, Value);
        }
    }
}
