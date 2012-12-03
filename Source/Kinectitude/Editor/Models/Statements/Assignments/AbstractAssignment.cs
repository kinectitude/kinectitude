using Kinectitude.Editor.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Statements.Assignments
{
    internal enum AssignmentOperator
    {
        Assign,
        Increment,
        Decrement,
        Divide,
        Multiply,
        ShiftRight,
        ShiftLeft,
        BitwiseAnd,
        BitwiseOr
    }

    internal abstract class AbstractAssignment : AbstractStatement
    {
        public abstract string Key { get; set; }
        public abstract AssignmentOperator Operator { get; set; }
        public abstract string Value { get; set; }

        protected AbstractAssignment(AbstractAssignment inheritedAssignment = null) : base(inheritedAssignment) { }

        public sealed override AbstractStatement DeepCopyStatement()
        {
            return new Assignment() { Key = this.Key, Operator = this.Operator, Value = this.Value };
        }

        public sealed override AbstractStatement CreateInheritor()
        {
            return new InheritedAssignment(this);
        }
    }
}
