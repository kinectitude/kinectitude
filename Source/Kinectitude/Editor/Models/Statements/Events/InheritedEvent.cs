using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Kinectitude.Editor.Models.Interfaces;
using System.Text.RegularExpressions;
using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models
{
    internal sealed class InheritedEvent : AbstractEvent
    {
        private readonly AbstractEvent inheritedEvent;
        private readonly Header header;

        public override Plugin Plugin
        {
            get { return inheritedEvent.Plugin; }
        }

        public override Header Header
        {
            get { return header; }
        }

        public InheritedEvent(AbstractEvent inheritedEvent) : base(inheritedEvent)
        {
            this.inheritedEvent = inheritedEvent;

            foreach (AbstractProperty inheritedProperty in inheritedEvent.Properties)
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
