using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Base
{
    internal sealed class Loop : Action
    {
        private readonly List<Action> Actions = new List<Action>();
        private readonly ValueReader Expression;

        internal Loop(ValueReader expr)
        {
            Expression = expr; 
        }

        public override void Run()
        {
            while (Expression)
            {
                foreach (Action a in Actions) a.Run();
            }
        }

        internal void AddAction(Action action)
        {
            Actions.Add(action);
        }
    }
}
