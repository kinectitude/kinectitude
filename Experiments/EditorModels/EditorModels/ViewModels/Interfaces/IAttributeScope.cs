
using System;
namespace EditorModels.ViewModels
{
    internal delegate void KeyEventHandler(string key);

    internal interface IAttributeScope
    {
        event KeyEventHandler InheritedAttributeAdded;
        event KeyEventHandler InheritedAttributeRemoved;
        event KeyEventHandler InheritedAttributeChanged;

        string GetInheritedValue(string key);
        bool HasInheritedAttribute(string key);
        bool HasLocalAttribute(string key);
    }
}
