//-----------------------------------------------------------------------
// <copyright file="ConditionGroup.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Statements.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Kinectitude.Editor.Models.Statements.Conditions
{
    internal sealed class ConditionGroup : AbstractConditionGroup
    {
        public ConditionGroup()
        {
            If = new ExpressionCondition();
        }

        public override void Accept(Storage.IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
