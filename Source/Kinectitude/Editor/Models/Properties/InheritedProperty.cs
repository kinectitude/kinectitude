using Kinectitude.Editor.Storage;
using System.Collections.Generic;
using System.Windows.Input;

namespace Kinectitude.Editor.Models.Properties
{
    internal sealed class InheritedProperty : AbstractProperty
    {
        private readonly AbstractProperty inheritedProperty;

        public override PluginProperty PluginProperty
        {
            get { return inheritedProperty.PluginProperty; }
        }

        public override object Value
        {
            get { return inheritedProperty.Value; }
            set { }
        }

        public override bool HasOwnValue
        {
            get { return inheritedProperty.HasOwnValue; }
        }

        public override IEnumerable<object> AvailableValues
        {
            get { return inheritedProperty.AvailableValues; }
        }

        public override ICommand ClearValueCommand { get; protected set; }

        public InheritedProperty(AbstractProperty inheritedProperty) : base(inheritedProperty)
        {
            this.inheritedProperty = inheritedProperty;
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
