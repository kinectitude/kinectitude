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
        private readonly string ifValue;
        private readonly List<LoadedBaseAction> actions = new List<LoadedBaseAction>();

        internal LoadedCondition(string ifVal) : base(null)
        {
            ifValue = ifVal;
        }

        internal override Action Create(Event evt)
        {
            BoolExpressionReader br = new BoolExpressionReader(ifValue, evt, evt.Entity);
            Condition condition = new Condition(br);
            foreach (LoadedAction loadedAction in actions)
            {
                Action action = loadedAction.Create(evt);
                condition.AddAction(action);
            }
            return condition;
        }


        internal void AddAction(LoadedAction action)
        {
            actions.Add(action);
        }

    }
}
