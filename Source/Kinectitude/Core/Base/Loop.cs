//-----------------------------------------------------------------------
// <copyright file="Loop.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Base
{
    internal sealed class Loop : Action
    {
        private readonly Action Before;
        private readonly List<Action> Actions = new List<Action>();
        private readonly ValueReader Expression;

        internal Loop(ValueReader expr, Action before)
        {
            Before = before;
            Expression = expr;
        }

        public override void Run()
        {
            if (null != Before)
            {
                Before.Run();
            }

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
