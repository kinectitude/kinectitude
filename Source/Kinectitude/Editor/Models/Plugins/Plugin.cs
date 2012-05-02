using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Models.Properties;

namespace Kinectitude.Editor.Models.Plugins
{
    public class Plugin
    {
        private readonly PluginDescriptor descriptor;
        private readonly SortedDictionary<string, BaseProperty> properties;
        
        public PluginDescriptor Descriptor
        {
            get { return descriptor; }
        }

        public IEnumerable<BaseProperty> Properties
        {
            get { return properties.Values; }
        }

        protected Plugin(PluginDescriptor descriptor)
        {
            this.descriptor = descriptor;
            properties = new SortedDictionary<string, BaseProperty>();
        }

        public virtual T GetProperty<T>(string name) where T : BaseProperty
        {
            return properties.ContainsKey(name) ? properties[name] as T : null;
        }

        public void SetProperty(string name, string value)
        {
            if (!properties.ContainsKey(name))
            {
                BaseProperty property = BaseProperty.CreateProperty(descriptor.GetPropertyDescriptor(name));
                if (null != property)
                {
                    properties[name] = property;
                }
            }

            if (properties.ContainsKey(name))
            {
                BaseProperty property = properties[name];
                property.TryParse(value);
            }
        }
    }
}
