using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Statements.Base;

namespace Kinectitude.Editor.Models.Statements.Conditions
{
    internal abstract class AbstractCondition : CompositeStatement, IStatementScope
    {
        public abstract string If { get; set; }

        protected AbstractCondition(AbstractCondition inheritedCondition = null) : base(inheritedCondition) { }

        public sealed override AbstractStatement DeepCopyStatement()
        {
            Condition copy = new Condition() { If = this.If };

            foreach (AbstractStatement statement in Statements)
            {
                copy.AddStatement(statement.DeepCopyStatement());
            }

            return copy;
        }

        public sealed override AbstractStatement CreateInheritor()
        {
            return new ReadOnlyCondition(this);
        }
    }
}
