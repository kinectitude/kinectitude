//-----------------------------------------------------------------------
// <copyright file="ReadOnlyEvent.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Statements.Base;
using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models.Statements.Events
{
    internal sealed class ReadOnlyEvent : AbstractEvent
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

        public ReadOnlyEvent(AbstractEvent inheritedEvent) : base(inheritedEvent)
        {
            this.inheritedEvent = inheritedEvent;

            foreach (AbstractProperty inheritedProperty in inheritedEvent.Properties)
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
