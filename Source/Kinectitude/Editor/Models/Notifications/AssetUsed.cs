using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Notifications
{
    internal sealed class AssetUsed : Notification
    {
        public string PathName { get; private set; }

        public AssetUsed(string pathName) : base(NotificationMode.Parent)
        {
            PathName = pathName;
        }
    }
}
