
namespace Kinectitude.Editor.Models.Interfaces
{
    internal delegate void ScopeChangedEventHandler();

    internal interface IScope
    {
        event ScopeChangedEventHandler ScopeChanged;
    }
}
