using Kinectitude.Editor.Models.Statements;
using Kinectitude.Editor.Storage;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Kinectitude.Editor.Models
{
    internal sealed class InheritedAction : AbstractAction
    {
        private readonly AbstractAction inheritedAction;
        private readonly Header header;

        public override Plugin Plugin
        {
            get { return inheritedAction.Plugin; }
        }

        public override Header Header
        {
            get { return header; }
        }

        public InheritedAction(AbstractAction inheritedAction) : base(inheritedAction)
        {
            this.inheritedAction = inheritedAction;

            foreach (AbstractProperty inheritedProperty in inheritedAction.Properties)
            {
                InheritedProperty localProperty = new InheritedProperty(inheritedProperty);
                AddProperty(localProperty);
            }

            header = new Header(Plugin.Header, Properties, true);
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
