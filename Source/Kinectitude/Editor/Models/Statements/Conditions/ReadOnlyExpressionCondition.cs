using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models.Statements.Conditions
{
    internal sealed class ReadOnlyExpressionCondition : AbstractExpressionCondition
    {
        private readonly AbstractExpressionCondition inheritedCondition;

        public override string Expression
        {
            get { return inheritedCondition.Expression; }
            set { }
        }

        public ReadOnlyExpressionCondition(AbstractExpressionCondition inheritedCondition) : base(inheritedCondition)
        {
            this.inheritedCondition = inheritedCondition;
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
