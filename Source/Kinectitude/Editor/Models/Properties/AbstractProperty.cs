using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;

namespace Kinectitude.Editor.Models
{
    internal abstract class AbstractProperty : GameModel<IPropertyScope>
    {
        public abstract PluginProperty PluginProperty { get; }

        public abstract string Name { get; }

        public abstract bool IsInherited { get; set; }

        [DependsOn("IsInherited")]
        public abstract bool IsLocal { get; }

        public abstract bool CanInherit { get; }

        [DependsOn("CanInherit")]
        public abstract bool IsRoot { get; }

        [DependsOn("IsInherited")]
        public abstract object Value { get; set; }

        protected AbstractProperty()
        {
            AddDependency<ScopeChanged>("CanInherit");
            AddDependency<ScopeChanged>("Value");
        }

        protected override void OnScopeDetaching(IPropertyScope scope)
        {
            scope.InheritedPropertyAdded -= OnInheritedPropertyAdded;
            scope.InheritedPropertyRemoved -= OnInheritedPropertyRemoved;
            scope.InheritedPropertyChanged -= OnInheritedPropertyChanged;
        }

        protected override void OnScopeAttaching(IPropertyScope scope)
        {
            scope.InheritedPropertyAdded += OnInheritedPropertyAdded;
            scope.InheritedPropertyRemoved += OnInheritedPropertyRemoved;
            scope.InheritedPropertyChanged += OnInheritedPropertyChanged;
        }

        private void OnInheritedPropertyAdded(PluginProperty property)
        {
            if (property.Name == Name)
            {
                NotifyPropertyChanged("Scope");
            }
        }

        private void OnInheritedPropertyRemoved(PluginProperty property)
        {
            if (property.Name == Name)
            {
                NotifyPropertyChanged("Scope");
            }
        }

        private void OnInheritedPropertyChanged(PluginProperty property)
        {
            if (property.Name == Name)
            {
                NotifyPropertyChanged("Scope");
            }
        }

        public Property DeepCopy()
        {
            return new Property(this.PluginProperty) { Value = this.Value };
        }
    }
}
