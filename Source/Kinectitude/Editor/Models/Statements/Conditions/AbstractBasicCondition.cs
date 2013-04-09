//-----------------------------------------------------------------------
// <copyright file="AbstractBasicCondition.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models.Statements.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Statements.Conditions
{
    internal abstract class AbstractBasicCondition : AbstractCondition
    {
        protected AbstractBasicCondition(AbstractBasicCondition sourceCondition = null) : base(sourceCondition) { }

        public sealed override AbstractStatement DeepCopyStatement()
        {
            return DeepCopyCondition();
        }

        public AbstractBasicCondition DeepCopyCondition()
        {
            BasicCondition copy = new BasicCondition();

            foreach (AbstractStatement statement in Statements)
            {
                copy.AddStatement(statement.DeepCopyStatement());
            }

            return copy;
        }

        public sealed override AbstractStatement CreateReadOnly()
        {
            return new ReadOnlyBasicCondition(this);
        }
    }
}
