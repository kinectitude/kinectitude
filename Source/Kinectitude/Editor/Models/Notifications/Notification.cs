using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Notifications
{
    internal class Notification
    {
        public enum NotificationMode { Children, Parent };

        public NotificationMode Mode { get; private set; }

        protected Notification(NotificationMode mode)
        {
            Mode = mode;
        }
    }
}
