using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Base;

namespace Kinectitude.Editor.Models.Plugins
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

        public override T GetProperty<T>(string name)
        {
            Property property = base.GetProperty<T>(name);

            if (null == property)
            {
                foreach (Entity prototype in parent.Prototypes)
                {
                    Component ancestor = prototype.GetComponent(Descriptor.Name);
                    if (null != ancestor)
                    {
                        property = ancestor.GetProperty<T>(name);
                        if (null != property)
                        {
                            break;
                        }
                    }
                }
            }
            return property as T;
        }
    }
}
