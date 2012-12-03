using Kinectitude.Editor.Models.Statements;
using Kinectitude.Editor.Storage;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Kinectitude.Editor.Models
{
    internal sealed class InheritedCondition : AbstractCondition
    {
        private readonly AbstractCondition inheritedCondition;

        public override string If
        {
            get { return inheritedCondition.If; }
            set { }
        }

        public InheritedCondition(AbstractCondition inheritedCondition) : base(inheritedCondition)
        {
            this.inheritedCondition = inheritedCondition;
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
