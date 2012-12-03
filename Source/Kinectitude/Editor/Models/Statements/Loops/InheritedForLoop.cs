using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models
{
    internal sealed class InheritedForLoop : AbstractForLoop
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

        public InheritedForLoop(AbstractForLoop inheritedLoop) : base(inheritedLoop)
        {
            this.inheritedLoop = inheritedLoop;
        }

        public override void Accept(Storage.IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
