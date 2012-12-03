using System;
using System.Collections.Generic;
using System.Linq;
using Kinectitude.Core.Attributes;
using Kinectitude.Editor.Base;
using System.Reflection;

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
        public string File { get; private set; }

        public PluginType Type { get; private set; }

        public Type CoreType { get; private set; }

        public string Provides { get; private set; }

        public IEnumerable<string> Requires { get; private set; }

        public string Header { get; private set; }

        public string Description { get; private set; }

        public string ClassName { get; private set; }

        public string ShortName { get; private set; }

        public IEnumerable<PluginProperty> Properties { get; private set; }

        public Plugin(Type type)
        {
            PluginAttribute pluginAttribute = System.Attribute.GetCustomAttribute(type, typeof(PluginAttribute)) as PluginAttribute;

            CoreType = type;
            File = type.Module.Name;
            Header = pluginAttribute.Header;
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

            List<PluginProperty> properties = new List<PluginProperty>();
            foreach (PropertyInfo info in type.GetProperties())
            {
                PluginPropertyAttribute pluginPropertyAttribute = System.Attribute.GetCustomAttribute(info, typeof(PluginPropertyAttribute)) as PluginPropertyAttribute;
                if (null != pluginPropertyAttribute)
                {
                    properties.Add(new PluginProperty(info, pluginPropertyAttribute));
                }
            }
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
