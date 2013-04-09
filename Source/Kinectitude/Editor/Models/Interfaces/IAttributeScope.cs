//-----------------------------------------------------------------------
// <copyright file="IAttributeScope.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------


using Kinectitude.Editor.Models.Values;
namespace Kinectitude.Editor.Models.Interfaces
{
    internal delegate void AttributeEventHandler(string key);

    internal interface IAttributeScope : IScope
    {
        Entity Entity { get; }
        Scene Scene { get; }
        Game Game { get; }

        event AttributeEventHandler InheritedAttributeAdded;
        event AttributeEventHandler InheritedAttributeRemoved;
        event AttributeEventHandler InheritedAttributeChanged;

        Value GetInheritedValue(string key);
        bool HasInheritedAttribute(string key);
        bool HasInheritedNonDefaultAttribute(string key);
        bool HasLocalAttribute(string key);

        
    }
}
