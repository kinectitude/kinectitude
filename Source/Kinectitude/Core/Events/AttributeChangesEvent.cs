using System.Linq;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using System;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Events
{
    // Use this as a base class for any attribute change.  It will always fire

    [Plugin("Expression {Target} changes", "")]
    internal sealed class AttributeChangesEvent : Event, IChangeable
    {
        [PluginProperty("Expression", "")]
        public ValueReader Target { get; set; }

        public AttributeChangesEvent() { }

        public override void OnInitialize()
        {
            Target.NotifyOfChange(this);
        }

        void IChangeable.Prepare() { }

        void IChangeable.Change() { DoActions(); }
    }
}
