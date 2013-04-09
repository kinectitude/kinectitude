//-----------------------------------------------------------------------
// <copyright file="Assignment.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Storage;

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
