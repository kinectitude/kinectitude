
using Kinectitude.Editor.Storage;
namespace Kinectitude.Editor.Models
{
    internal sealed class InheritedProperty : AbstractProperty
    {
        private readonly AbstractProperty inheritedProperty;

        public override PluginProperty PluginProperty
        {
            get { return inheritedProperty.PluginProperty; }
        }

        public override string Name
        {
            get { return inheritedProperty.Name; }
        }

        public override bool IsInherited
        {
            get { return inheritedProperty.IsInherited; }
            set { }
        }

        public override bool IsLocal
        {
            get { return inheritedProperty.IsLocal; }
        }

        public override bool CanInherit
        {
            get { return inheritedProperty.CanInherit; }
        }

        public override bool IsRoot
        {
            get { return inheritedProperty.IsRoot; }
        }

        public override object Value
        {
            get { return inheritedProperty.Value; }
            set { }
        }

        public InheritedProperty(AbstractProperty inheritedProperty)
        {
            this.inheritedProperty = inheritedProperty;
            inheritedProperty.PropertyChanged += (sender, args) => NotifyPropertyChanged(args.PropertyName);
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
