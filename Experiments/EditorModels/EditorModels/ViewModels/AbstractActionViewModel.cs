using System.Collections.Generic;
using EditorModels.ViewModels.Interfaces;
using System.Linq;

namespace EditorModels.ViewModels
{
    internal abstract class AbstractActionViewModel : BaseViewModel, IPropertyScope
    {
        private readonly List<AbstractPropertyViewModel> properties;
        protected IActionScope scope;

        public event ScopeChangedEventHandler ScopeChanged;
        public event PropertyEventHandler InheritedPropertyAdded { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyRemoved { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyChanged { add { } remove { } }
        
        public abstract string Type { get; }

        public abstract bool IsLocal { get; }

        public abstract bool IsInherited { get; }

        public abstract IEnumerable<PluginViewModel> Plugins { get; }

        public IEnumerable<AbstractPropertyViewModel> Properties
        {
            get { return properties; }
        }

        protected AbstractActionViewModel()
        {
            properties = new List<AbstractPropertyViewModel>();
        }

        protected void AddProperty(AbstractPropertyViewModel property)
        {
            property.SetScope(this);
            properties.Add(property);
        }

        public AbstractPropertyViewModel GetProperty(string name)
        {
            return Properties.FirstOrDefault(x => x.Name == name);
        }

        public void SetProperty(string name, object value)
        {
            if (IsLocal)
            {
                AbstractPropertyViewModel property = GetProperty(name);
                if (null != property)
                {
                    property.Value = value;
                }
            }
        }

        public void SetScope(IActionScope scope)
        {
            if (null != this.scope)
            {
                this.scope.ScopeChanged -= OnScopeChanged;
                this.scope.DefineAdded -= OnDefineAdded;
                this.scope.DefineChanged -= OnDefinedNameChanged;
            }

            this.scope = scope;

            if (null != this.scope)
            {
                this.scope.ScopeChanged += OnScopeChanged;
                this.scope.DefineAdded += OnDefineAdded;
                this.scope.DefineChanged += OnDefinedNameChanged;
            }

            NotifyPropertyChanged("Scope");
        }

        private void OnScopeChanged()
        {
            if (null != ScopeChanged)
            {
                ScopeChanged();
            }
        }

        private void OnDefineAdded(DefineViewModel define)
        {
            NotifyPropertyChanged("Scope");
        }

        private void OnDefinedNameChanged(PluginViewModel plugin, string newName)
        {
            NotifyPropertyChanged("Scope");
        }

        bool IPropertyScope.HasInheritedProperty(string name)
        {
            return false;
        }

        object IPropertyScope.GetInheritedValue(string name)
        {
            return null;
        }

        public abstract bool InheritsFrom(AbstractActionViewModel action);
    }
}
