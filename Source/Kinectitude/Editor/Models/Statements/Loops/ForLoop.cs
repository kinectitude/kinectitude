using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models.Statements.Loops
{
    internal sealed class ForLoop : AbstractForLoop
    {
        private string preExpression;
        private string expression;
        private string postExpression;

        public override string PreExpression
        {
            get { return preExpression; }
            set
            {
                if (preExpression != value)
                {
                    var oldPreExpression = preExpression;

                    Workspace.Instance.CommandHistory.Log(
                        "change for loop initializer",
                        () => PreExpression = value,
                        () => PreExpression = oldPreExpression
                    );

                    preExpression = value;
                    NotifyPropertyChanged("PreExpression");
                }
            }
        }

        public override string Expression
        {
            get { return expression; }
            set
            {
                if (expression != value)
                {
                    var oldExpression = expression;

                    Workspace.Instance.CommandHistory.Log(
                        "change for loop expression",
                        () => Expression = value,
                        () => Expression = oldExpression
                    );

                    expression = value;
                    NotifyPropertyChanged("Expression");
                }
            }
        }

        public override string PostExpression
        {
            get { return postExpression; }
            set
            {
                if (postExpression != value)
                {
                    var oldPostExpression = postExpression;

                    Workspace.Instance.CommandHistory.Log(
                        "change for loop updater",
                        () => PostExpression = value,
                        () => PostExpression = oldPostExpression
                    );

                    postExpression = value;
                    NotifyPropertyChanged("PostExpression");
                }
            }
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
