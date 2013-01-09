namespace Kinectitude.Editor.Models.Interfaces
{
    internal interface IPluginNamespace
    {
        string GetDefinedName(Plugin plugin);
        Plugin GetPlugin(string name);
    }
}
