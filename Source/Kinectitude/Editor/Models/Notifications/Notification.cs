
namespace Kinectitude.Editor.Models.Notifications
{
    internal class Notification
    {
        public enum NotificationMode { Children, Parent };

        public NotificationMode Mode { get; private set; }

        public bool Handled { get; set; }

        protected Notification(NotificationMode mode)
        {
            Mode = mode;
        }
    }
}
