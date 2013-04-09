//-----------------------------------------------------------------------
// <copyright file="AbstractCondition.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Statements.Base;

namespace Kinectitude.Editor.Models.Statements.Conditions
{
    internal abstract class AbstractCondition : CompositeStatement
    {
        public virtual bool HasExpression
        {
            get { return false; }
        }

        protected AbstractCondition(AbstractCondition inheritedCondition) : base(inheritedCondition) { }
    }
}
