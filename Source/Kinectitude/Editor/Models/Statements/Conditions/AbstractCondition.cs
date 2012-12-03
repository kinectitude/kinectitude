using System.Collections.ObjectModel;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Statements;

namespace Kinectitude.Editor.Models
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
            return new InheritedCondition(this);
        }
    }
}
