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

        public T GetProperty<T>(string name) where T : Property
        {
            Property ret;
            properties.TryGetValue(name, out ret);
            return ret as T;
        }

        public void SetProperty(string name, string value)
        {
            Property property;
            properties.TryGetValue(name, out property);

            if (null == property)
            {
                property = Property.CreateProperty(descriptor.GetPropertyDescriptor(name));
                if (null != property)
                {
                    properties[name] = property;
                }
            }

            if (null != property)
            {
                property.TryParse(value);
            }
        }
    }
}
