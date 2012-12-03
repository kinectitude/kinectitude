using Kinectitude.Editor.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Statements.Assignments
{
    internal sealed class InheritedAssignment : AbstractAssignment
    {
        private readonly AbstractAssignment inheritedAssignment;

        public override string Key
        {
            get { return inheritedAssignment.Key; }
            set { }
        }

        public override AssignmentOperator Operator
        {
            get { return inheritedAssignment.Operator; }
            set { }
        }

        public override string Value
        {
            get { return inheritedAssignment.Value; }
            set { }
        }

        public InheritedAssignment(AbstractAssignment inheritedAssignment) : base(inheritedAssignment)
        {
            this.inheritedAssignment = inheritedAssignment;
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
