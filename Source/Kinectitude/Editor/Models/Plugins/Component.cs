using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Editor
{
    public class Component : Plugin
    {
        private Entity parent;

        public Entity Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public Component(PluginDescriptor descriptor) : base(descriptor) { }

        public override string ToString()
        {
            return string.Format("Type: {0}", Descriptor.Name);
        }
    }
}
