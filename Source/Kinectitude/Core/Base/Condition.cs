using System.Collections.Generic;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Base
{
    internal class Condition : Action
    {
        private List<Action> actions = new List<Action>();
        private IBoolExpressionReader expression;

        internal Condition(IBoolExpressionReader expr)
        {
            expression = expr;
        }

        internal void AddAction(Action action)
        {
            actions.Add(action);
        }

        internal bool ShouldRun()
        {
            return expression.GetValue();
        }
        
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
