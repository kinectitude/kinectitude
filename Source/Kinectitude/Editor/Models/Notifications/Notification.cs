//-----------------------------------------------------------------------
// <copyright file="Notification.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------


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
