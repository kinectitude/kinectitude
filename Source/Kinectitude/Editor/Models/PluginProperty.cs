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
        private readonly Value defaultValue;

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
        public string FileFilter
        {
            get { return attribute.FileFilter; }
        }

        public string FileChooserTitle
        {
            get { return attribute.FileChooserTitle; }
        }

        public Value DefaultValue
        {
            get { return defaultValue; }
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

            object rawDefault = attribute.DefaultValue;

            if (null == rawDefault)
            {
                if (type.IsValueType)
                {
                    rawDefault = Activator.CreateInstance(type);
                }
            }

            defaultValue = new Value(rawDefault, true);
        }
    }
}
