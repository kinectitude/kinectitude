
namespace Kinectitude.Editor.ViewModels.Interfaces
{
    internal delegate void DefineAddedEventHandler(DefineViewModel define);
    internal delegate void DefinedNameChangedEventHandler(PluginViewModel plugin, string newName);

    internal interface IPluginNamespace
    {
        event DefineAddedEventHandler DefineAdded;
        event DefinedNameChangedEventHandler DefineChanged;

        string GetDefinedName(PluginViewModel plugin);
        PluginViewModel GetPlugin(string name);
    }
}
