using Kinectitude.Core.Attributes;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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

        public object DefaultValue
        {
            get { return attribute.DefaultValue; }
        }

        public PluginProperty(PropertyInfo info, PluginPropertyAttribute attribute)
        {
            this.info = info;
            this.attribute = attribute;
        }

        public Value CreateValue()
        {
            Type type = info.PropertyType;

            if (type.IsEnum)
            {
                return new EnumerationValue(type);
            }
            else if (type == typeof(bool))
            {
                return new BooleanValue();
            }

            return new StringValue();
        }
    }
}
