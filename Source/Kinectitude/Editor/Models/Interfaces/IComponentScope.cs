//-----------------------------------------------------------------------
// <copyright file="IComponentScope.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------


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
        bool HasInheritedNonDefaultProperty(Plugin plugin, PluginProperty property);
        Value GetInheritedValue(Plugin plugin, PluginProperty property);
    }
}
