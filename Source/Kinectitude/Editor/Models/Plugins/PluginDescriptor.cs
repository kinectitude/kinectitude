using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Kinectitude.Core.Attributes;
using Kinectitude.Editor.Models.Properties;

namespace Kinectitude.Editor.Models.Plugins
{
    public class PluginDescriptor
    {
        public enum PluginType
        {
            Component,
            Event,
            Action
        }

        private readonly string name;
        private readonly string file;
        private readonly string displayName;
        private readonly PluginType type;
        private readonly string description;
        private readonly List<PropertyDescriptor> propertyDescriptors;

        public string Name
        {
            get { return name; }
        }

        public string File
        {
            get { return file; }
        }

        public string DisplayName
        {
            get { return displayName; }
        }

        public PluginType Type
        {
            get { return type; }
        }

        public string Description
        {
            get { return description; }
        }

        public IEnumerable<PropertyDescriptor> PropertyDescriptors
        {
            get { return propertyDescriptors; }
        }

        public PluginDescriptor(Type pluginType)
        {
            name = pluginType.FullName;
            file = pluginType.Module.Name;
            displayName = pluginType.Name;
            
            PropertyInfo[] propertyInfo = pluginType.GetProperties();
            propertyDescriptors = new List<PropertyDescriptor>();

            PluginAttribute pluginAttribute = System.Attribute.GetCustomAttribute(pluginType, typeof(PluginAttribute)) as PluginAttribute;

            if (null != pluginAttribute)
            {
                displayName = pluginAttribute.Name;
                description = pluginAttribute.Description;
            }
            
            if (typeof(Kinectitude.Core.Base.Component).IsAssignableFrom(pluginType))
            {
                type = PluginType.Component;
            }
            else if (typeof(Kinectitude.Core.Base.Event).IsAssignableFrom(pluginType))
            {
                type = PluginType.Event;
            }
            else if (typeof(Kinectitude.Core.Base.Action).IsAssignableFrom(pluginType))
            {
                type = PluginType.Action;
            }

            foreach (PropertyInfo property in propertyInfo)
            {
                if (System.Attribute.IsDefined(property, typeof(PluginAttribute)))
                {
                    PropertyDescriptor descriptor = new PropertyDescriptor(property);
                    propertyDescriptors.Add(descriptor);
                }
            }
        }

        public PropertyDescriptor GetPropertyDescriptor(string name)
        {
            return propertyDescriptors.FirstOrDefault(x => x.Name == name);
        }
    }
}
