
using System.Collections.Generic;
namespace EditorModels.ViewModels
{
    internal abstract class AbstractActionViewModel
    {
        public abstract string Type { get; }

        public abstract IEnumerable<PluginViewModel> Plugins { get; }

        public abstract IEnumerable<PropertyViewModel> Properties { get; }
    }
}
