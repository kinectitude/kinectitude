using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Notifications
{
    internal sealed class EffectiveValueChanged : Notification
    {
        public Plugin Plugin { get; private set; }

        public PluginProperty PluginProperty { get; private set; }

        public EffectiveValueChanged(Plugin plugin, PluginProperty property) : base(NotificationMode.Parent)
        {
            Plugin = plugin;
            PluginProperty = property;
        }
    }
}
