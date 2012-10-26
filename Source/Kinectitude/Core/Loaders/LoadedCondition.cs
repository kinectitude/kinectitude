using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Loaders
{
    class LoadedCondition : LoadedBaseAction
    {
        private readonly object ifValue;
        private readonly List<LoadedBaseAction> actions = new List<LoadedBaseAction>();

        internal LoadedCondition(object ifVal, LoaderUtility loaderUtil) : base(null, loaderUtil)
        {
            ifValue = ifVal;
        }

        internal override Action Create(Event evt)
        {
            Condition condition = new Condition(LoaderUtil.MakeAssignable(ifValue, evt.Entity.Scene, evt.Entity, evt) as ValueReader);
            foreach (LoadedBaseAction loadedAction in actions)
            {
                Action action = loadedAction.Create(evt);
                condition.AddAction(action);
            }
            return condition;
        }

        internal void AddAction(LoadedBaseAction action)
        {
            actions.Add(action);
        }

    }
}
