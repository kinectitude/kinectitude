using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EditorModels.ViewModels.Interfaces;

namespace EditorModels.ViewModels
{
    internal class ActionViewModel : AbstractActionViewModel
    {
        private readonly PluginViewModel plugin;

        public override IEnumerable<PluginViewModel> Plugins
        {
            get { return Enumerable.Repeat(plugin, 1); }
        }

        [DependsOn("Scope")]
        public override string Type
        {
            get { return null != scope ? scope.GetDefinedName(plugin) : plugin.ClassName; }
        }

        public override bool IsInherited
        {
            get { return false; }
        }

        public override bool IsLocal
        {
            get { return true; }
        }

        public ActionViewModel(PluginViewModel plugin)
        {
            this.plugin = plugin;

            foreach (string property in plugin.Properties)
            {
                AddProperty(new PropertyViewModel(property));
            }
        }

        public override bool InheritsFrom(AbstractActionViewModel action)
        {
            return false;
        }
    }
}
