//-----------------------------------------------------------------------
// <copyright file="ReadOnlyExpressionCondition.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models.Statements.Conditions
{
    internal sealed class ReadOnlyExpressionCondition : AbstractExpressionCondition
    {
        private readonly AbstractExpressionCondition inheritedCondition;

        public override string Expression
        {
            get { return inheritedCondition.Expression; }
            set { }
        }

        public ReadOnlyExpressionCondition(AbstractExpressionCondition inheritedCondition) : base(inheritedCondition)
        {
            this.inheritedCondition = inheritedCondition;
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
