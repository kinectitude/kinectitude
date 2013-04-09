//-----------------------------------------------------------------------
// <copyright file="PluginUsed.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------


namespace Kinectitude.Editor.Models.Notifications
{
    internal sealed class PluginUsed : Notification
    {
        public Plugin Plugin { get; private set; }

        public PluginUsed(Plugin plugin) : base(NotificationMode.Parent)
        {
            Plugin = plugin;
        }
    }
}
