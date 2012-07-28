﻿
namespace Kinectitude.Editor.ViewModels.Interfaces
{
    internal delegate void ComponentEventHandler(PluginViewModel plugin);

    internal interface IComponentScope : IScope, IPluginNamespace
    {
        event ComponentEventHandler InheritedComponentAdded;
        event ComponentEventHandler InheritedComponentRemoved;
        event PropertyEventHandler InheritedPropertyChanged;

        bool HasInheritedComponent(PluginViewModel plugin);
        bool HasRootComponent(PluginViewModel plugin);

        object GetInheritedValue(PluginViewModel plugin, string name);
    }
}