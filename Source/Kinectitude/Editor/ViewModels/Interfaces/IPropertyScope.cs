
namespace Kinectitude.Editor.ViewModels.Interfaces
{
    internal delegate void PropertyEventHandler(string name);

    internal interface IPropertyScope : IScope
    {
        event PropertyEventHandler InheritedPropertyAdded;
        event PropertyEventHandler InheritedPropertyRemoved;
        event PropertyEventHandler InheritedPropertyChanged;

        bool HasInheritedProperty(string name);
        object GetInheritedValue(string name);
    }
}
