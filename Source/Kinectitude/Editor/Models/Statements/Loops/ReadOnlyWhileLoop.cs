//-----------------------------------------------------------------------
// <copyright file="ReadOnlyWhileLoop.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------


namespace Kinectitude.Editor.Models.Statements.Loops
{
    internal sealed class ReadOnlyWhileLoop : AbstractWhileLoop
    {
        private readonly AbstractWhileLoop inheritedLoop;

        public override string Expression
        {
            get { return inheritedLoop.Expression; }
            set { }
        }

        public ReadOnlyWhileLoop(AbstractWhileLoop inheritedLoop) : base(inheritedLoop)
        {
            this.inheritedLoop = inheritedLoop;
        }

        public override void Accept(Storage.IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
