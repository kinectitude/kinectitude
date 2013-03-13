namespace Kinectitude.Editor.Models.Interfaces
{
    internal interface IPluginNamespace
    {
        bool HasDefinedName(string name);
        string GetDefinedName(Plugin plugin);
        Plugin GetPlugin(string name);
    }
}
