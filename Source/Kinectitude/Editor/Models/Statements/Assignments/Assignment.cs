using Kinectitude.Editor.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Statements.Assignments
{
    internal sealed class Assignment : AbstractAssignment
    {
        private string key;
        private AssignmentOperator op;
        private string val;

        public override string Key
        {
            get { return key; }
            set
            {
                if (key != value)
                {
                    key = value;
                    NotifyPropertyChanged("Key");
                }
            }
        }

        public override AssignmentOperator Operator
        {
            get { return op; }
            set
            {
                if (op != value)
                {
                    op = value;
                    NotifyPropertyChanged("Operator");
                }
            }
        }

        public override string Value
        {
            get { return val; }
            set
            {
                if (val != value)
                {
                    val = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
