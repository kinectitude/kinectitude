//-----------------------------------------------------------------------
// <copyright file="INotify.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models.Notifications;

namespace Kinectitude.Editor.Models.Interfaces
{
    internal interface INotify
    {
        void Notify<T>(GameModel source, T e) where T : Notification;
    }
}
