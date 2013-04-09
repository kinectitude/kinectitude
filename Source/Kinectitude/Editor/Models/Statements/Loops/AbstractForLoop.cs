//-----------------------------------------------------------------------
// <copyright file="AbstractForLoop.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models.Statements.Base;

namespace Kinectitude.Editor.Models.Statements.Loops
{
    internal abstract class AbstractForLoop : CompositeStatement
    {
        public abstract string PreExpression { get; set; }
        public abstract string Expression { get; set; }
        public abstract string PostExpression { get; set; }

        public AbstractForLoop(AbstractForLoop inheritedLoop = null) : base(inheritedLoop) { }

        public sealed override AbstractStatement DeepCopyStatement()
        {
            ForLoop copy = new ForLoop()
            {
                PreExpression = this.PreExpression,
                Expression = this.Expression,
                PostExpression = this.PostExpression
            };

            foreach (AbstractStatement statement in Statements)
            {
                copy.AddStatement(statement.DeepCopyStatement());
            }

            return copy;
        }

        public sealed override AbstractStatement CreateReadOnly()
        {
            return new ReadOnlyForLoop(this);
        }
    }
}
