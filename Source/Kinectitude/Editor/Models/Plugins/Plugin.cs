using System.Collections.Generic;
using Kinectitude.Editor.Models.Properties;

namespace Kinectitude.Editor.Models.Plugins
{
    internal class Plugin
    {
        private readonly PluginDescriptor descriptor;
        private readonly SortedDictionary<string, Property> properties;
        
        public PluginDescriptor Descriptor
        {
            get { return descriptor; }
        }

        public IEnumerable<Property> Properties
        {
            get { return properties.Values; }
        }

        protected Plugin(PluginDescriptor descriptor)
        {
            this.descriptor = descriptor;
            properties = new SortedDictionary<string, Property>();
        }

        public virtual T GetProperty<T>(string name) where T : Property
        {
            return properties.ContainsKey(name) ? properties[name] as T : null;
        }

        public void SetProperty(string name, string value)
        {
            if (!properties.ContainsKey(name))
            {
                Property property = Property.CreateProperty(descriptor.GetPropertyDescriptor(name));
                if (null != property)
                {
                    properties[name] = property;
                }
            }

            if (properties.ContainsKey(name))
            {
                Property property = properties[name];
                property.TryParse(value);
            }
        }
    }
}
