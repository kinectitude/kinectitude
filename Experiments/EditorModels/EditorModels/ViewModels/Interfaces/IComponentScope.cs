using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.ViewModels.Interfaces
{
    interface IComponentScope : IScope, IPluginNamespace
    {
        bool HasInheritedComponent(PluginViewModel plugin);
        bool HasRootComponent(PluginViewModel plugin);
    }
}
