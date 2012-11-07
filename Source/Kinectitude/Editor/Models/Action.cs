using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kinectitude.Editor.Models
{
    internal class Action : AbstractAction
    {
        private readonly Plugin plugin;

        public override IEnumerable<Plugin> Plugins
        {
            get { return Enumerable.Repeat(plugin, 1); }
        }

        public override Plugin Plugin
        {
            get { return plugin; }
        }

        [DependsOn("Scope")]
        public override string Type
        {
            get { return null != scope ? scope.GetDefinedName(plugin) : plugin.ClassName; }
        }

        public override string DisplayName
        {
            get { return plugin.Header; }
        }

        public IEnumerable<object> Tokens { get; private set; }

        public override bool IsInherited
        {
            get { return false; }
        }

        public override bool IsLocal
        {
            get { return true; }
        }

        public Action(Plugin plugin)
        {
            if (plugin.Type != PluginType.Action)
            {
                throw new ArgumentException("Plugin is not an action");
            }

            this.plugin = plugin;

            foreach (string property in plugin.Properties)
            {
                AddProperty(new Property(property));
            }

            string[] splitHeader = Regex.Split(plugin.Header, "({.*?})");
            List<object> tokens = new List<object>();

            foreach (string token in splitHeader)
            {
                if (token.StartsWith("{"))
                {
                    string property = token.TrimStart('{').TrimEnd('}');
                    tokens.Add(GetProperty(property));
                }
                else if (token != string.Empty)
                {
                    tokens.Add(token);
                }
            }

            Tokens = tokens;
        }

        public override bool InheritsFrom(AbstractAction action)
        {
            return false;
        }
    }
}
