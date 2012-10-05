using System;
using System.Collections.Generic;
using System.Linq;

namespace Kinectitude.Editor.Models
{
    internal class Action : AbstractAction
    {
        private readonly Plugin plugin;

        public override IEnumerable<Plugin> Plugins
        {
            get { return Enumerable.Repeat(plugin, 1); }
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
        }

        public override bool InheritsFrom(AbstractAction action)
        {
            return false;
        }
    }
}
