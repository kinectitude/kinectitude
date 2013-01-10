using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models.Statements.Conditions
{
    internal class Condition : AbstractCondition
    {
        private string rule;

        public override string If
        {
            get { return rule; }
            set
            {
                if (rule != value)
                {
                    rule = value;
                    NotifyPropertyChanged("If");
                }
            }
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
