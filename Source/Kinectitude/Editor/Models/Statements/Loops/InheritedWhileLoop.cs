using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models
{
    internal sealed class InheritedWhileLoop : AbstractWhileLoop
    {
        private readonly AbstractWhileLoop inheritedLoop;

        public override string Expression
        {
            get { return inheritedLoop.Expression; }
            set { }
        }

        public InheritedWhileLoop(AbstractWhileLoop inheritedLoop) : base(inheritedLoop)
        {
            this.inheritedLoop = inheritedLoop;
        }

        public override void Accept(Storage.IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
