using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Statements.Base;

namespace Kinectitude.Editor.Models.Statements.Conditions
{
    internal abstract class AbstractCondition : CompositeStatement, IStatementScope
    {
        public abstract string Expression { get; set; }

        protected AbstractCondition(AbstractCondition inheritedCondition = null) : base(inheritedCondition) { }

        public sealed override AbstractStatement DeepCopyStatement()
        {
            return DeepCopyCondition();
        }

        public AbstractCondition DeepCopyCondition()
        {
            Condition copy = new Condition() { Expression = this.Expression };

            foreach (AbstractStatement statement in Statements)
            {
                copy.AddStatement(statement.DeepCopyStatement());
            }

            return copy;
        }

        public sealed override AbstractStatement CreateReadOnly()
        {
            return new ReadOnlyCondition(this);
        }
    }
}
