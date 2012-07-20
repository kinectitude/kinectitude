
namespace Kinectitude.Editor.ViewModels.Interfaces
{
    internal delegate void ScopeChangedEventHandler();

    internal interface IScope
    {
        event ScopeChangedEventHandler ScopeChanged;
    }
}
