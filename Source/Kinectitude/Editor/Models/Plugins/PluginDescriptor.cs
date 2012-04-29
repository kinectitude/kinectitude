using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Kinectitude.Attributes;

namespace Editor
{
    public class PluginDescriptor
    {
        public enum PluginType
        {
            Component,
            Event,
            Action
        }

        private readonly string className;
        private readonly string name;
        private readonly PluginType type;
        private readonly string description;
        //private readonly Uri image;
        private readonly List<PropertyDescriptor> propertyDescriptors;

        public string Class
        {
            get { return className; }
        }

        public string Name
        {
            get { return name; }
        }

        public PluginType Type
        {
            get { return type; }
        }

        public string Description
        {
            get { return description; }
        }

        /*public Uri Image
        {
            get { return image; }
        }*/

        public IEnumerable<PropertyDescriptor> PropertyDescriptors
        {
            get { return propertyDescriptors; }
        }

        public PluginDescriptor(Type pluginType)
        {
            className = pluginType.Name;

            PropertyInfo[] propertyInfo = pluginType.GetProperties();
            propertyDescriptors = new List<PropertyDescriptor>();

            PluginAttribute pluginAttribute = Attribute.GetCustomAttribute(pluginType, typeof(PluginAttribute)) as PluginAttribute;

            if (null != pluginAttribute)
            {
                name = pluginAttribute.Name;
                description = pluginAttribute.Description;
                //image = new Uri(pluginAttribute.Icon);
            }
            else
            {
                name = pluginType.Name;
            }
            
            if (typeof(Kinectitude.Core.Component).IsAssignableFrom(pluginType))
            {
                type = PluginType.Component;
            }
            else if (typeof(Kinectitude.Core.Event).IsAssignableFrom(pluginType))
            {
                type = PluginType.Event;
            }
            else if (typeof(Kinectitude.Core.Action).IsAssignableFrom(pluginType))
            {
                type = PluginType.Action;
            }

            foreach (PropertyInfo property in propertyInfo)
            {
                if (Attribute.IsDefined(property, typeof(PluginAttribute)))
                {
                    PropertyDescriptor descriptor = new PropertyDescriptor(property);
                    propertyDescriptors.Add(descriptor);
                }
            }
        }
    }
}
