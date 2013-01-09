
namespace Kinectitude.Editor.Models.Notifications
{
    internal sealed class PluginUsed : Notification
    {
        public Plugin Plugin { get; private set; }

        public PluginUsed(Plugin plugin) : base(NotificationMode.Parent)
        {
            Plugin = plugin;
        }
    }
}
