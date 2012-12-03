using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Notifications
{
    internal sealed class ScopeChanged : Notification
    {
        public ScopeChanged() : base(NotificationMode.Children) { }
    }
}
