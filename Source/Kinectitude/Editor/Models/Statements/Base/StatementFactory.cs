//-----------------------------------------------------------------------
// <copyright file="StatementFactory.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Kinectitude.Editor.Models.Statements.Base
{
    internal enum StatementType
    {
        Event,
        Action,
        Assignment,
        ConditionGroup,
        Condition,
        WhileLoop,
        ForLoop
    }

    internal sealed class StatementFactory
    {
        private readonly string name;
        private readonly StatementType type;
        private readonly Func<AbstractStatement> function;
        
        public string Name
        {
            get { return name; }
        }

        public StatementType Type
        {
            get { return type; }
        }

        public StatementFactory(string name, StatementType type, Func<AbstractStatement> function)
        {
            this.name = name;
            this.type = type;
            this.function = function;
        }

        public AbstractStatement CreateStatement()
        {
            return function();
        }
    }
}
