
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
