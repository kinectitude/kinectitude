
using Kinectitude.Editor.Models.Values;
namespace Kinectitude.Editor.Models.Interfaces
{
    internal delegate void ComponentEventHandler(Plugin plugin);

    internal interface IComponentScope : IScope, IPluginNamespace
    {
        event ComponentEventHandler InheritedComponentAdded;
        event ComponentEventHandler InheritedComponentRemoved;
        event PropertyEventHandler InheritedPropertyChanged;

        bool HasInheritedComponent(Plugin plugin);
        bool HasRootComponent(Plugin plugin);
        bool HasInheritedNonDefaultValue(Plugin plugin, PluginProperty property);
        Value GetInheritedValue(Plugin plugin, PluginProperty property);
    }
}
