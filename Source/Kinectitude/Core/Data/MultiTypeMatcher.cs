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
