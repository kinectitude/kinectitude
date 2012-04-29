using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public class Action : Plugin
    {
        private Event parent;

        public Event Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public Action(PluginDescriptor descriptor) : base(descriptor) { }

        public override string ToString()
        {
            return string.Format("Type: {0}", Descriptor.Name);
        }
    }
}
