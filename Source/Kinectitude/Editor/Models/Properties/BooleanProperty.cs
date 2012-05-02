using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Properties
{
    public class BooleanProperty : Property<bool>
    {
        public BooleanProperty(PropertyDescriptor descriptor) : base(descriptor) { }

        public override bool TryParse(string input)
        {
            bool parsed = false;
            bool ret = bool.TryParse(input, out parsed);
            Value = parsed;
            return ret;
        }
    }
}
