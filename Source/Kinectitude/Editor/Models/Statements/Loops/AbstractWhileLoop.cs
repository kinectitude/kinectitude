//-----------------------------------------------------------------------
// <copyright file="AbstractWhileLoop.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models.Statements.Base;

namespace Kinectitude.Editor.Models.Statements.Loops
{
    internal abstract class AbstractWhileLoop : CompositeStatement
    {
        public abstract string Expression { get; set; }

        public AbstractWhileLoop(AbstractWhileLoop inheritedLoop = null) : base(inheritedLoop) { }

        public sealed override AbstractStatement DeepCopyStatement()
        {
            WhileLoop copy = new WhileLoop() { Expression = this.Expression };

            foreach (AbstractStatement statement in Statements)
            {
                copy.AddStatement(statement.DeepCopyStatement());
            }

            return copy;
        }

        public sealed override AbstractStatement CreateReadOnly()
        {
            return new ReadOnlyWhileLoop(this);
        }
    }
}
