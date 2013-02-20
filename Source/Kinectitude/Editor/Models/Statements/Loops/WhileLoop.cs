using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models.Statements.Loops
{
    internal sealed class WhileLoop : AbstractWhileLoop
    {
        private string expression;

        public override string Expression
        {
            get { return expression; }
            set
            {
                if (expression != value)
                {
                    var oldExpression = expression;

                    Workspace.Instance.CommandHistory.Log(
                        "change loop expression",
                        () => Expression = value,
                        () => Expression = oldExpression
                    );

                    expression = value;
                    NotifyPropertyChanged("Expression");
                }
            }
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}