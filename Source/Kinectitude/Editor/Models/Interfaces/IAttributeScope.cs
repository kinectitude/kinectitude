﻿
namespace Kinectitude.Editor.Models.Interfaces
{
    internal delegate void AttributeEventHandler(string key);

    internal interface IAttributeScope : IScope
    {
        event AttributeEventHandler InheritedAttributeAdded;
        event AttributeEventHandler InheritedAttributeRemoved;
        event AttributeEventHandler InheritedAttributeChanged;

        object GetInheritedValue(string key);
        bool HasInheritedAttribute(string key);
        bool HasLocalAttribute(string key);
    }
}
