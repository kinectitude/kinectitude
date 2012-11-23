using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Loaders
{
    internal sealed class LoadedCondition : LoadedActionable
    {
        private readonly LoadedCondition ElseCond;

        internal LoadedCondition(object ifVal, LoadedCondition elseCond, LoaderUtility loaderUtil) : base(ifVal, loaderUtil)
        {
            ElseCond = elseCond;
        }

        internal override Action Create(Event evt)
        {
            Condition cond = null;
            if (ElseCond != null) cond = ElseCond.Create(evt) as Condition;
            ValueReader reader = LoaderUtil.MakeAssignable(ConditianlExpression, evt.Entity.Scene, evt.Entity, evt) as ValueReader;
            Condition condition = new Condition(reader, cond);
            condition.Event = evt;
            foreach (LoadedBaseAction loadedAction in Actions)
            {
                Action action = loadedAction.Create(evt);
                condition.AddAction(action);
            }
            return condition;
        }
    }
}
