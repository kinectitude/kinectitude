
namespace Kinectitude.Editor.Models.Notifications
{
    internal sealed class DefineAdded : Notification
    {
        public Define Define { get; private set; }

        public DefineAdded(Define define) : base(NotificationMode.Children)
        {
            Define = define;
        }
    }
}
