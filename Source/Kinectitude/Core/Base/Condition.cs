using System.Collections.Generic;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Base
{
    internal class Condition : Action
    {
        private List<Action> actions = new List<Action>();
        private ValueReader expression;

        internal Condition(ValueReader expr)
        {
            expression = expr;
        }

        internal void AddAction(Action action)
        {
            actions.Add(action);
        }

        internal bool ShouldRun() { return expression.GetBoolValue(); }
        
        public override void Run()
        {
            if (ShouldRun())
            {
                foreach (Action a in actions)
                {
                    a.Run();
                }
            }
        }
    }
}
