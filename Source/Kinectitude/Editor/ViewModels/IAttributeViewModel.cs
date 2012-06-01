
namespace Kinectitude.Editor.ViewModels
{
    internal interface IAttributeViewModel
    {
        string Key { get; set; }
        string Value { get; set; }
        bool CanInherit { get; }
        bool IsInherited { get; set; }
        bool IsLocal { get; }

        void AddAttribute();
        void RemoveAttribute();
    }
}
