using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Statements.Conditions
{
    internal class ReadOnlyBasicCondition : AbstractBasicCondition
    {
        public ReadOnlyBasicCondition(AbstractBasicCondition sourceCondition) : base(sourceCondition) { }

        public override void Accept(Storage.IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
