
namespace Kinectitude.Editor.Models.Transactions
{
    internal sealed class PluginSelection
    {
        public Plugin Plugin { get; private set; }
        public bool IsRequired { get; private set; }

        public PluginSelection(Plugin plugin, bool required)
        {
            Plugin = plugin;
            IsRequired = required;
        }
    }
}
