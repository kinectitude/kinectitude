using System.Collections.Generic;
using System.Linq;
using Kinectitude.Editor.ViewModels.Interfaces;

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class EventViewModel : AbstractEventViewModel
    {
        private readonly PluginViewModel plugin;

        public override event DefineAddedEventHandler DefineAdded;
        public override event DefinedNameChangedEventHandler DefineChanged;

        [DependsOn("Scope")]
        public override string Type
        {
            get { return null != scope ? scope.GetDefinedName(plugin) : plugin.ClassName; }
        }

        public override string DisplayName
        {
            get { return plugin.DisplayName; }
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

        public override IEnumerable<PluginViewModel> Plugins
        {
            get { return Actions.SelectMany(x => x.Plugins).Union(Enumerable.Repeat(plugin, 1)).Distinct(); }
        }

        public EventViewModel(PluginViewModel plugin)
        {
            this.plugin = plugin;

            foreach (string property in plugin.Properties)
            {
                AddProperty(new PropertyViewModel(property));
            }
        }

        public override bool InheritsFrom(AbstractEventViewModel evt)
        {
            return false;
        }
    }
}
