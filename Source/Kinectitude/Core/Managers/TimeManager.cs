//-----------------------------------------------------------------------
// <copyright file="TimeManager.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Managers
{
    /// <summary>
    /// A basic manager used to update components on how much time has passed since the last update.
    /// </summary>
    [Plugin("Time Manager", "")]
    public class TimeManager : Manager<Component>
    {
        public TimeManager() { }

        public override void OnUpdate(float t)
        {
            foreach (Component c in Children)
            {
                c.OnUpdate(t);
            }
        }
    }
}
