﻿using Kinectitude.Editor.Models.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models
{
    internal abstract class AbstractForLoop : CompositeStatement
    {
        public abstract string PreExpression { get; set; }
        public abstract string Expression { get; set; }
        public abstract string PostExpression { get; set; }

        public AbstractForLoop(AbstractForLoop inheritedLoop = null) : base(inheritedLoop) { }

        public sealed override AbstractStatement DeepCopyStatement()
        {
            ForLoop copy = new ForLoop()
            {
                PreExpression = this.PreExpression,
                Expression = this.Expression,
                PostExpression = this.PostExpression
            };

            foreach (AbstractStatement statement in Statements)
            {
                copy.AddStatement(statement.DeepCopyStatement());
            }

            return copy;
        }

        public sealed override AbstractStatement CreateInheritor()
        {
            return new InheritedForLoop(this);
        }
    }
}