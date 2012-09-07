using System;
using System.Collections.Generic;
using System.Linq;
using Kinectitude.Core.Attributes;
using Kinectitude.Editor.Base;

namespace Kinectitude.Editor.Models
{
    public enum PluginType
    {
        Manager,
        Component,
        Event,
        Action
    }

    internal sealed class Plugin : BaseModel
    {
        public string File
        {
            get;
            private set;
        }

        public PluginType Type
        {
            get;
            private set;
        }

        public Type CoreType
        {
            get;
            private set;
        }

        public string Provides
        {
            get;
            private set;
        }

        public IEnumerable<string> Requires
        {
            get;
            private set;
        }

        public string DisplayName
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public string ClassName
        {
            get;
            private set;
        }

        public string ShortName
        {
            get;
            private set;
        }

        public IEnumerable<string> Properties
        {
            get;
            private set;
        }

        public Plugin(Type type)
        {
            CoreType = type;
            
            PluginAttribute pluginAttribute = System.Attribute.GetCustomAttribute(type, typeof(PluginAttribute)) as PluginAttribute;

            File = type.Module.Name;
            DisplayName = pluginAttribute.Name;
            Description = pluginAttribute.Description;
            ClassName = type.FullName;
            ShortName = type.Name;
            Provides = ClassName;

            if (typeof(Kinectitude.Core.Base.IManager).IsAssignableFrom(type))
            {
                Type = PluginType.Manager;
            }
            else if (typeof(Kinectitude.Core.Base.Component).IsAssignableFrom(type))
            {
                Type = PluginType.Component;
            }
            else if (typeof(Kinectitude.Core.Base.Event).IsAssignableFrom(type))
            {
                Type = PluginType.Event;
            }
            else if (typeof(Kinectitude.Core.Base.Action).IsAssignableFrom(type))
            {
                Type = PluginType.Action;
            }

            var properties = from property in type.GetProperties() where System.Attribute.IsDefined(property, typeof(PluginAttribute)) select property.Name;
            Properties = properties.ToArray();

            ProvidesAttribute providesAttribute = System.Attribute.GetCustomAttribute(type, typeof(ProvidesAttribute)) as ProvidesAttribute;
            if (null != providesAttribute)
            {
                Provides = providesAttribute.Type.FullName;
            }

            List<string> requires = new List<string>();
            System.Attribute[] requiresAttributes = System.Attribute.GetCustomAttributes(type, typeof(RequiresAttribute));
            foreach (RequiresAttribute attribute in requiresAttributes)
            {
                requires.Add(attribute.Type.FullName);
            }
            Requires = requires.ToArray();
        }
    }
}
