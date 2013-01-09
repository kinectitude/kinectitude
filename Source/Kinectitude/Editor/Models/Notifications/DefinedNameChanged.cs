
namespace Kinectitude.Editor.Models.Notifications
{
    internal sealed class DefinedNameChanged : Notification
    {
        public Plugin Plugin { get; private set; }

        public DefinedNameChanged(Plugin plugin) : base(NotificationMode.Children)
        {
            Plugin = plugin;
        }
    }
}
