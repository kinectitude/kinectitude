//-----------------------------------------------------------------------
// <copyright file="DefineRemoved.cs" company="Kinectitude">
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
    internal sealed class DefineRemoved : Notification
    {
        public Define Define { get; private set; }

        public DefineRemoved(Define define) : base(NotificationMode.Children)
        {
            Define = define;
        }
    }
}
