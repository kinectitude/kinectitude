
namespace Kinectitude.Editor.Models.Notifications
{
    internal sealed class DefinedNameChanged : Notification
    {
        public string OldName { get; private set; }

        public Plugin Plugin { get; private set; }

        public DefinedNameChanged(string oldName, Plugin plugin) : base(NotificationMode.Children)
        {
            OldName = oldName;
            Plugin = plugin;
        }
    }
}
