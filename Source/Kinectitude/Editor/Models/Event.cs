using System;
using System.Collections.Generic;
using System.Linq;
using Kinectitude.Editor.Models.Interfaces;

namespace Kinectitude.Editor.Models
{
    internal sealed class Event : AbstractEvent
    {
        private readonly Plugin plugin;

        public override event DefineAddedEventHandler DefineAdded;
        public override event DefinedNameChangedEventHandler DefineChanged;

        [DependsOn("Scope")]
        public override string Type
        {
            get { return null != scope ? scope.GetDefinedName(plugin) : plugin.ClassName; }
        }

        public override string Header
        {
            get { return plugin.Header; }
        }

        public override string Description
        {
            get { return plugin.Description; }
        }

        [DependsOn("IsInherited")]
        public override bool IsLocal
        {
            get { return !IsInherited; }
        }

        public override bool IsInherited
        {
            get { return false; }
        }

        public override IEnumerable<Plugin> Plugins
        {
            get { return Actions.SelectMany(x => x.Plugins).Union(Enumerable.Repeat(plugin, 1)).Distinct(); }
        }

        public Event(Plugin plugin)
        {
            if (plugin.Type != PluginType.Event)
            {
                throw new ArgumentException("Plugin is not an event");
            }

            this.plugin = plugin;

            foreach (string property in plugin.Properties)
            {
                AddProperty(new Property(property));
            }
        }

        public override bool InheritsFrom(AbstractEvent evt)
        {
            return false;
        }
    }
}
