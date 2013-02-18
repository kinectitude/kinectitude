using Kinectitude.Core.Attributes;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kinectitude.Editor.Models
{
    internal class PluginProperty : BaseModel
    {
        private readonly PropertyInfo info;
        private readonly PluginPropertyAttribute attribute;

        public string Name
        {
            get { return info.Name; }
        }

        public string DisplayName
        {
            get { return attribute.Name; }
        }

        public string Description
        {
            get { return attribute.Description; }
        }

        // Whether or not we can use windows explorer to look for a file. 
        // Used for images and sound
        public bool UsesFileSelector
        {
            get { return attribute.UsesFileSelector; }
        }

        public Value DefaultValue
        {
            get
            {
                object result = attribute.DefaultValue;

                if (null == result)
                {
                    Type type = info.PropertyType;

                    if (type.IsValueType)
                    {
                        result = Activator.CreateInstance(type);
                    }
                }

                return new Value(result, true);
            }
        }

        public IEnumerable<object> AvailableValues { get; private set; }

        public PluginProperty(PropertyInfo info, PluginPropertyAttribute attribute)
        {
            this.info = info;
            this.attribute = attribute;

            Type type = info.PropertyType;

            if (type.IsEnum)
            {
                AvailableValues = Enum.GetNames(type).Select(x => "\"" + x + "\"");
            }
            else if (type == typeof(bool))
            {
                AvailableValues = new object[] { true, false };
            }
            else
            {
                AvailableValues = Enumerable.Empty<object>();
            }
        }
    }
}
