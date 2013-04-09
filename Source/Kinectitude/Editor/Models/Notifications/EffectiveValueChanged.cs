//-----------------------------------------------------------------------
// <copyright file="EffectiveValueChanged.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

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
