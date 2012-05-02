using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Properties
{
    public class TextProperty : Property<string>
    {
        public TextProperty(PropertyDescriptor descriptor) : base(descriptor) { }

        public override bool TryParse(string input)
        {
            Value = null != input ? input : string.Empty;
            return true;
        }
    }
}
