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
        private readonly object IfValue;
        private readonly List<LoadedBaseAction> Actions = new List<LoadedBaseAction>();
        private readonly LoadedCondition ElseCond;

        internal LoadedCondition(object ifVal, LoadedCondition elseCond, LoaderUtility loaderUtil) : base(null, loaderUtil)
        {
            IfValue = ifVal;
            ElseCond = elseCond;
        }

        internal override Action Create(Event evt)
        {
            Condition cond = null;
            if (ElseCond != null) cond = ElseCond.Create(evt) as Condition;
            ValueReader reader = LoaderUtil.MakeAssignable(IfValue, evt.Entity.Scene, evt.Entity, evt) as ValueReader;
            Condition condition = new Condition(reader, cond);
            foreach (LoadedBaseAction loadedAction in Actions)
            {
                Action action = loadedAction.Create(evt);
                condition.AddAction(action);
            }
            return condition;
        }

        internal void AddAction(LoadedBaseAction action)
        {
            Actions.Add(action);
        }

    }
}
