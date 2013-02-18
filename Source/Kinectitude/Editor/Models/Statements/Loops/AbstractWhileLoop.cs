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
