
namespace Kinectitude.Editor.Models.Interfaces
{
    internal delegate void PropertyEventHandler(PluginProperty property);

    internal interface IPropertyScope : IScope
    {
        event PropertyEventHandler InheritedPropertyAdded;
        event PropertyEventHandler InheritedPropertyRemoved;
        event PropertyEventHandler InheritedPropertyChanged;

        bool HasInheritedProperty(string name);
        object GetInheritedValue(string name);
    }
}
