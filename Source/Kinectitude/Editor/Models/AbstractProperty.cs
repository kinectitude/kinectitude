using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;

namespace Kinectitude.Editor.Models
{
    internal abstract class AbstractProperty : VisitableModel
    {
        protected IPropertyScope scope;

        public abstract string Name { get; }

        public abstract bool IsInherited { get; set; }

        [DependsOn("IsInherited")]
        public abstract bool IsLocal { get; }

        [DependsOn("Scope")]
        public abstract bool CanInherit { get; }

        [DependsOn("CanInherit")]
        public abstract bool IsRoot { get; }

        [DependsOn("IsInherited")]
        [DependsOn("Scope")]
        public abstract object Value { get; set; }

        public void SetScope(IPropertyScope scope)
        {
            if (null != this.scope)
            {
                this.scope.InheritedPropertyAdded -= OnInheritedPropertyAdded;
                this.scope.InheritedPropertyRemoved -= OnInheritedPropertyRemoved;
                this.scope.InheritedPropertyChanged -= OnInheritedPropertyChanged;
            }

            this.scope = scope;

            if (null != this.scope)
            {
                this.scope.InheritedPropertyAdded += OnInheritedPropertyAdded;
                this.scope.InheritedPropertyRemoved += OnInheritedPropertyRemoved;
                this.scope.InheritedPropertyChanged += OnInheritedPropertyChanged;
            }

            NotifyPropertyChanged("Scope");
        }

        private void OnInheritedPropertyAdded(string name)
        {
            if (name == Name)
            {
                NotifyPropertyChanged("Scope");
            }
        }

        private void OnInheritedPropertyRemoved(string name)
        {
            if (name == Name)
            {
                NotifyPropertyChanged("Scope");
            }
        }

        private void OnInheritedPropertyChanged(string name)
        {
            if (name == Name)
            {
                NotifyPropertyChanged("Scope");
            }
        }

        public Property DeepCopy()
        {
            return new Property(this.Name) { Value = this.Value };
        }
    }
}
