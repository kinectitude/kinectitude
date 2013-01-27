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
            If = new Condition();
        }

        public override void Accept(Storage.IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
