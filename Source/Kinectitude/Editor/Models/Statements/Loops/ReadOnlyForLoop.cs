//-----------------------------------------------------------------------
// <copyright file="ReadOnlyForLoop.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------


namespace Kinectitude.Editor.Models.Statements.Loops
{
    internal sealed class ReadOnlyForLoop : AbstractForLoop
    {
        private readonly AbstractForLoop inheritedLoop;

        public override string PreExpression
        {
            get { return inheritedLoop.PreExpression; }
            set { }
        }

        public override string Expression
        {
            get { return inheritedLoop.Expression; }
            set { }
        }

        public override string PostExpression
        {
            get { return inheritedLoop.PostExpression; }
            set { }
        }

        public ReadOnlyForLoop(AbstractForLoop inheritedLoop) : base(inheritedLoop)
        {
            this.inheritedLoop = inheritedLoop;
        }

        public override void Accept(Storage.IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
