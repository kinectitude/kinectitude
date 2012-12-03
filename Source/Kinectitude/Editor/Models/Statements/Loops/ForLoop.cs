using Kinectitude.Editor.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models
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
