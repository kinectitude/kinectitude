
namespace Kinectitude.Editor.Models.Interfaces
{
    internal interface IActionScope : IScope, IPluginNamespace
    {
        void InsertBefore(AbstractAction action, AbstractAction toInsert);
    }
}
