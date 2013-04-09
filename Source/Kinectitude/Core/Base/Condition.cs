//-----------------------------------------------------------------------
// <copyright file="Condition.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Base
{
    internal sealed class Condition : Action
    {
        private readonly List<Action> Actions = new List<Action>();
        private readonly Condition ElseCond;
        private readonly ValueReader Expression;


        internal Condition(ValueReader expr, Condition elseCond)
        {
            Expression = expr;
            ElseCond = elseCond;
        }

        internal void AddAction(Action action)
        {
            Actions.Add(action);
        }
        
        public override void Run()
        {
            if (Expression)
            {
                foreach (Action a in Actions) a.Run();
            }
            else if(ElseCond != null)
            {
                ElseCond.Run();
            }
        }
    }
}
