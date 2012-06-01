using Kinectitude.Editor.Models.Plugins;

namespace Kinectitude.Editor.Storage
{
    internal interface IPluginNamespace
    {
        PluginDescriptor GetPluginDescriptor(string name);
    }
}
