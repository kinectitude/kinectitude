using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Kinectitude.Attributes;

namespace Editor
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

        private readonly string propertyName;
        private readonly string key;
        private readonly string description;
        private readonly PropertyType type;
        private readonly string[] enumeration;

        public string PropertyName
        {
            get { return propertyName; }
        }

        public string Key
        {
            get { return key; }
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
            propertyName = propertyInfo.Name;

            PluginAttribute pluginPropertyAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(PluginAttribute)) as PluginAttribute;

            if (null != pluginPropertyAttribute)
            {
                key = pluginPropertyAttribute.Name;
                description = pluginPropertyAttribute.Description;
            }
            else
            {
                key = propertyInfo.Name;
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
