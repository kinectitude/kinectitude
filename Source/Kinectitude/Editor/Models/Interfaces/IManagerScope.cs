
namespace Kinectitude.Editor.Models.Interfaces
{
    internal interface IManagerScope : IScope, IPluginNamespace
    {
        bool RequiresManager(Manager manager);
    }
}
