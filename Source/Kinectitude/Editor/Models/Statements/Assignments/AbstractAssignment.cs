//-----------------------------------------------------------------------
// <copyright file="AbstractAssignment.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Language;
using Kinectitude.Editor.Models.Statements.Base;
using System.Collections.Generic;

namespace Kinectitude.Editor.Models.Statements.Assignments
{
    internal enum AssignmentOperator
    {
        Assign,
        Increment,
        Decrement,
        Divide,
        Multiply,
        Remainder,
        Power,
        ShiftRight,
        ShiftLeft,
        And,
        Or
    }

    internal abstract class AbstractAssignment : AbstractStatement
    {
        internal static readonly Dictionary<KinectitudeGrammar.OpCode, AssignmentOperator> AssignmentValues =
            new Dictionary<KinectitudeGrammar.OpCode, AssignmentOperator>()
            {
                {KinectitudeGrammar.OpCode.Becomes, AssignmentOperator.Assign},
                {KinectitudeGrammar.OpCode.Plus, AssignmentOperator.Increment},
                {KinectitudeGrammar.OpCode.Minus, AssignmentOperator.Decrement},
                {KinectitudeGrammar.OpCode.Div, AssignmentOperator.Divide},
                {KinectitudeGrammar.OpCode.Mult, AssignmentOperator.Multiply},
                {KinectitudeGrammar.OpCode.Rem, AssignmentOperator.Remainder},
                {KinectitudeGrammar.OpCode.Pow, AssignmentOperator.Power},
                {KinectitudeGrammar.OpCode.RightShift, AssignmentOperator.ShiftRight},
                {KinectitudeGrammar.OpCode.LeftShift, AssignmentOperator.ShiftLeft},
                {KinectitudeGrammar.OpCode.And, AssignmentOperator.And},
                {KinectitudeGrammar.OpCode.Or, AssignmentOperator.Or}
            };
        public abstract string Key { get; set; }
        public abstract AssignmentOperator Operator { get; set; }
        public abstract string Value { get; set; }

        protected AbstractAssignment(AbstractAssignment inheritedAssignment = null) : base(inheritedAssignment) { }

        public sealed override AbstractStatement DeepCopyStatement()
        {
            return new Assignment() { Key = this.Key, Operator = this.Operator, Value = this.Value };
        }

        public sealed override AbstractStatement CreateReadOnly()
        {
            return new ReadOnlyAssignment(this);
        }
    }
}
