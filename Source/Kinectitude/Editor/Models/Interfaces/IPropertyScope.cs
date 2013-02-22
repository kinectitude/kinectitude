using Kinectitude.Editor.Models.Values;
namespace Kinectitude.Editor.Models.Interfaces
{
    internal delegate void PropertyEventHandler(PluginProperty property);

    internal interface IPropertyScope : IScope
    {
        event PropertyEventHandler InheritedPropertyAdded;
        event PropertyEventHandler InheritedPropertyRemoved;
        event PropertyEventHandler InheritedPropertyChanged;

        bool HasInheritedProperty(PluginProperty property);
        bool HasInheritedNonDefaultValue(PluginProperty PluginProperty);
        Value GetInheritedValue(PluginProperty property);
    }
}
