using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Kinectitude.Attributes;

namespace Kinectitude.Editor.Models.Properties
{
    public class PropertyDescriptor
    {
        public enum PropertyType
        {
            Text,
            Integer,
            Real,
            Boolean,
            Enumeration,
            Expression,
            Asset
        }

        private readonly string name;
        private readonly string displayName;
        private readonly string description;
        private readonly PropertyType type;
        private readonly string[] enumeration;

        public string Name
        {
            get { return name; }
        }

        public string DisplayName
        {
            get { return displayName; }
        }

        public string Description
        {
            get { return description; }
        }

        public PropertyType Type
        {
            get { return type; }
        }

        public IEnumerable<string> Enumeration
        {
            get { return enumeration; }
        }

        public PropertyDescriptor(PropertyInfo propertyInfo)
        {
            name = propertyInfo.Name;

            PluginAttribute pluginPropertyAttribute = System.Attribute.GetCustomAttribute(propertyInfo, typeof(PluginAttribute)) as PluginAttribute;

            if (null != pluginPropertyAttribute)
            {
                displayName = pluginPropertyAttribute.Name;
                description = pluginPropertyAttribute.Description;
            }
            else
            {
                displayName = propertyInfo.Name;
            }

            Type propertyType = propertyInfo.PropertyType;

            if (propertyType == typeof(string))
            {
                type = PropertyType.Text;
            }
            else if (propertyType == typeof(int))
            {
                type = PropertyType.Integer;
            }
            else if (propertyType == typeof(double))
            {
                type = PropertyType.Real;
            }
            else if (propertyType == typeof(bool))
            {
                type = PropertyType.Boolean;
            }
            else if (propertyType.IsEnum)
            {
                type = PropertyType.Enumeration;
                enumeration = Enum.GetNames(propertyType);
            }
        }
    }
}
