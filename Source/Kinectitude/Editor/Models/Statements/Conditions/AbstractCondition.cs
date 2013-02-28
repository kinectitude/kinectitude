using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Statements.Base;

namespace Kinectitude.Editor.Models.Statements.Conditions
{
    internal abstract class AbstractCondition : CompositeStatement
    {
        public virtual bool HasExpression
        {
            get { return false; }
        }

        protected AbstractCondition(AbstractCondition inheritedCondition) : base(inheritedCondition) { }
    }
}
