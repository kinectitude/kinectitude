using System.ComponentModel;
using EditorModels.ViewModels.Interfaces;

namespace EditorModels.ViewModels
{
    internal sealed class PropertyViewModel : BaseViewModel
    {
        private readonly string name;
        private object value;
        private bool inherited;
        private IPropertyScope scope;

        public string Name
        {
            get { return name; }
        }

        public bool IsInherited
        {
            get { return inherited; }
            set
            {
                if (inherited != value)
                {
                    inherited = value;
                    NotifyPropertyChanged("IsInherited");
                }
            }
        }

        [DependsOn("IsInherited")]
        public bool IsLocal
        {
            get { return !IsInherited; }
        }

        [DependsOn("Scope")]
        public bool CanInherit
        {
            get { return null != scope ? scope.HasInheritedProperty(Name) : false; }
        }

        [DependsOn("CanInherit")]
        public bool IsRoot
        {
            get { return !CanInherit; }
        }

        [DependsOn("IsInherited")]
        [DependsOn("Scope")]
        public object Value
        {
            get
            {
                if (IsInherited)
                {
                    return null != scope ? scope.GetInheritedValue(Name) : 0;   // TODO: Get actual default
                }

                return value;
            }
            set
            {
                if (this.value != value)
                {
                    if (IsRoot || IsLocal)
                    {
                        this.value = value;  // TODO: Serialize anything
                        IsInherited = false;
                        NotifyPropertyChanged("Value");
                    }
                }
            }
        }

        public PropertyViewModel(string name)
        {
            this.name = name;
            this.inherited = true;
        }

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
    }
}
