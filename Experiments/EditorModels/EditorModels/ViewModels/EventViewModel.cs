using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EditorModels.ViewModels.Interfaces;

namespace EditorModels.ViewModels
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
            get { return Actions.SelectMany(x => x.Plugins).Union(Enumerable.Repeat(plugin, 1)); }
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
