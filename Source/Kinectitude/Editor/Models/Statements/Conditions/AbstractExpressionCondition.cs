//-----------------------------------------------------------------------
// <copyright file="AbstractExpressionCondition.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models.Statements.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Statements.Conditions
{
    internal abstract class AbstractExpressionCondition : AbstractCondition
    {
        public override bool HasExpression
        {
            get { return true; }
        }

        public abstract string Expression { get; set; }

        protected AbstractExpressionCondition(AbstractExpressionCondition inheritedCondition = null) : base(inheritedCondition) { }

        public sealed override AbstractStatement DeepCopyStatement()
        {
            return DeepCopyCondition();
        }

        public AbstractExpressionCondition DeepCopyCondition()
        {
            ExpressionCondition copy = new ExpressionCondition() { Expression = this.Expression };

            foreach (AbstractStatement statement in Statements)
            {
                copy.AddStatement(statement.DeepCopyStatement());
            }

            return copy;
        }

        public sealed override AbstractStatement CreateReadOnly()
        {
            return new ReadOnlyExpressionCondition(this);
        }
    }
}
