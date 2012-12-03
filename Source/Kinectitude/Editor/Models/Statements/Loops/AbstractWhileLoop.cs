using Kinectitude.Editor.Models.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models
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

        public sealed override AbstractStatement CreateInheritor()
        {
            return new InheritedWhileLoop(this);
        }
    }
}
