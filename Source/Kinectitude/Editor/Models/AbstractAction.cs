using System.Collections.Generic;
using System.Linq;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;

namespace Kinectitude.Editor.Models
{
    internal abstract class AbstractAction : BaseModel, IPropertyScope
    {
        private readonly List<AbstractProperty> properties;
        protected IActionScope scope;

        public event ScopeChangedEventHandler ScopeChanged;
        public event PropertyEventHandler InheritedPropertyAdded { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyRemoved { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyChanged { add { } remove { } }
        
        public abstract string Type { get; }

        public abstract string DisplayName { get; }

        public abstract bool IsLocal { get; }

        public abstract bool IsInherited { get; }

        public abstract IEnumerable<Plugin> Plugins { get; }

        public IEnumerable<AbstractProperty> Properties
        {
            get { return properties; }
        }

        protected AbstractAction()
        {
            properties = new List<AbstractProperty>();
        }

        protected void AddProperty(AbstractProperty property)
        {
            property.SetScope(this);
            properties.Add(property);
        }

        public AbstractProperty GetProperty(string name)
        {
            return Properties.FirstOrDefault(x => x.Name == name);
        }

        public void SetProperty(string name, object value)
        {
            if (IsLocal)
            {
                AbstractProperty property = GetProperty(name);
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

        private void OnDefineAdded(Define define)
        {
            NotifyPropertyChanged("Scope");
        }

        private void OnDefinedNameChanged(Plugin plugin, string newName)
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

        public abstract bool InheritsFrom(AbstractAction action);
    }
}
