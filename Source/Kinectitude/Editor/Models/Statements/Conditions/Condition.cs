using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Statements;
using Kinectitude.Editor.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Kinectitude.Editor.Models
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
