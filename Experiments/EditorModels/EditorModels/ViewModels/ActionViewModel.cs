using System.Collections.Generic;
using System.Linq;

namespace EditorModels.ViewModels
{
    internal class ActionViewModel : BaseViewModel
    {
        private readonly PluginViewModel plugin;

        public virtual IEnumerable<PluginViewModel> Plugins
        {
            get { return Enumerable.Repeat(plugin, 1); }
        }

        public ActionViewModel(PluginViewModel plugin)
        {
            this.plugin = plugin;
        }
    }
}
