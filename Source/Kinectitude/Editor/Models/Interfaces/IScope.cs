using Kinectitude.Editor.Models.Notifications;
using System.Collections.Generic;

namespace Kinectitude.Editor.Models.Interfaces
{
    internal interface IScope
    {
        IScope Parent { get; }
        IList<GameModel> Children { get; }

        void Notify<T>(T notification) where T : Notification;
    }
}
