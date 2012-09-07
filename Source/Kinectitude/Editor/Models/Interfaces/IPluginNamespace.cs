
namespace Kinectitude.Editor.Models.Interfaces
{
    internal delegate void DefineAddedEventHandler(Define define);
    internal delegate void DefinedNameChangedEventHandler(Plugin plugin, string newName);

    internal interface IPluginNamespace
    {
        event DefineAddedEventHandler DefineAdded;
        event DefinedNameChangedEventHandler DefineChanged;

        string GetDefinedName(Plugin plugin);
        Plugin GetPlugin(string name);
    }
}
