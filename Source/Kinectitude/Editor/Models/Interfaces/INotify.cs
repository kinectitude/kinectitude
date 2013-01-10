using Kinectitude.Editor.Models.Notifications;

namespace Kinectitude.Editor.Models.Interfaces
{
    internal interface INotify
    {
        void Notify<T>(GameModel source, T e) where T : Notification;
    }
}
