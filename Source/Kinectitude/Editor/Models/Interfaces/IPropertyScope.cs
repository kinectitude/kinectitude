//-----------------------------------------------------------------------
// <copyright file="IPropertyScope.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

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
