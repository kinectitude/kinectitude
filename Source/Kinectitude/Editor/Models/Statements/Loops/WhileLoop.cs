using Kinectitude.Editor.Models.Statements;
using Kinectitude.Editor.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models
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