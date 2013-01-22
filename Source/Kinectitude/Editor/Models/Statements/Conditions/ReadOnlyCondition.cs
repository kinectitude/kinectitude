using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models.Statements.Conditions
{
    internal sealed class ReadOnlyCondition : AbstractCondition
    {
        private readonly AbstractCondition inheritedCondition;

        public override string If
        {
            get { return inheritedCondition.If; }
            set { }
        }

        public ReadOnlyCondition(AbstractCondition inheritedCondition) : base(inheritedCondition)
        {
            this.inheritedCondition = inheritedCondition;
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
