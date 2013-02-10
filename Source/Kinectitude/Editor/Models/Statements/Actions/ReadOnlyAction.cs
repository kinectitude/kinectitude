using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Statements.Base;
using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models.Statements.Actions
{
    internal sealed class ReadOnlyAction : AbstractAction
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

        public ReadOnlyAction(AbstractAction inheritedAction) : base(inheritedAction)
        {
            this.inheritedAction = inheritedAction;

            foreach (AbstractProperty inheritedProperty in inheritedAction.Properties)
            {
                ReadOnlyProperty localProperty = new ReadOnlyProperty(inheritedProperty);
                AddProperty(localProperty);
            }

            header = new Header(Plugin.Header, Properties, false);
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
