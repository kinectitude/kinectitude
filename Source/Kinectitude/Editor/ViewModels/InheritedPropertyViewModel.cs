
namespace Kinectitude.Editor.ViewModels
{
    internal sealed class InheritedPropertyViewModel : AbstractPropertyViewModel
    {
        private readonly AbstractPropertyViewModel inheritedProperty;

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

        public InheritedPropertyViewModel(AbstractPropertyViewModel inheritedProperty)
        {
            this.inheritedProperty = inheritedProperty;
            inheritedProperty.PropertyChanged += (sender, args) => NotifyPropertyChanged(args.PropertyName);
        }
    }
}
