using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public class Plugin
    {
        private readonly PluginDescriptor descriptor;
        private readonly BaseProperty[] properties;
        
        public PluginDescriptor Descriptor
        {
            get { return descriptor; }
        }

        public IEnumerable<BaseProperty> Properties
        {
            get { return properties; }
        }

        protected Plugin(PluginDescriptor descriptor)
        {
            this.descriptor = descriptor;
            PropertyDescriptor[] propertyDescriptors = descriptor.PropertyDescriptors.ToArray();
            properties = new BaseProperty[propertyDescriptors.Length];

            int i = 0;
            foreach (PropertyDescriptor propertyDescriptor in propertyDescriptors)
            {
                BaseProperty property = BaseProperty.CreateProperty(propertyDescriptor);

                if (null != property)
                {
                    properties[i++] = property;
                }
            }
        }

        public BaseProperty GetProperty(string key)
        {
            return properties.FirstOrDefault(x => x.Descriptor.Key == key);
        }
    }
}
