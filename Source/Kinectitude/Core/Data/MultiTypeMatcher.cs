//-----------------------------------------------------------------------
// <copyright file="MultiTypeMatcher.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal abstract class MultiTypeMatcher : TypeMatcher
    {
        protected readonly List<Action<DataContainer>> notify = new List<Action<DataContainer>>();

        internal override void NotifyOfChange(Action<DataContainer> action)
        {
            notify.Add(action);
        }
    }
}
