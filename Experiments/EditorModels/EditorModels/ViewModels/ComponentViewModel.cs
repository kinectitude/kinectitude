using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using EditorModels.ViewModels.Interfaces;

namespace EditorModels.ViewModels
{
    internal sealed class ComponentViewModel : BaseViewModel, IPropertyScope
    {
        private readonly PluginViewModel plugin;
        private readonly List<PropertyViewModel> properties;
        private IComponentScope scope;

        public event ScopeChangedEventHandler ScopeChanged;
        public event PropertyEventHandler InheritedPropertyAdded;
        public event PropertyEventHandler InheritedPropertyRemoved;
        public event PropertyEventHandler InheritedPropertyChanged;
        public event PropertyEventHandler LocalPropertyChanged;

        public PluginViewModel Plugin
        {
            get { return plugin; }
        }

        public string DisplayName
        {
            get { return plugin.DisplayName; }
        }

        [DependsOn("Scope")]
        public string Type
        {
            get { return null != scope ? scope.GetDefinedName(plugin) : plugin.ClassName; }
        }

        public string Provides
        {
            get { return plugin.Provides; }
        }

        public IEnumerable<string> Requires
        {
            get { return plugin.Requires; }
        }

        public bool HasLocalProperties
        {
            get { return Properties.Any(x => !x.IsInherited); }
        }

        [DependsOn("IsRoot")]
        public bool IsInherited
        {
            get { return !IsRoot; }
        }

        [DependsOn("Scope")]
        public bool IsRoot
        {
            get { return null != scope ? !scope.HasInheritedComponent(plugin) : true; }
        }

        public IEnumerable<PropertyViewModel> Properties
        {
            get { return properties; }
        }

        public ComponentViewModel(PluginViewModel plugin)
        {
            this.plugin = plugin;
            
            properties = new List<PropertyViewModel>();
            foreach (string property in plugin.Properties)
            {
                AddProperty(new PropertyViewModel(property));
            }
        }

        private void AddProperty(PropertyViewModel property)
        {
            property.SetScope(this);
            property.PropertyChanged += OnPropertyValueChanged;
            properties.Add(property);
        }

        public void SetScope(IComponentScope scope)
        {
            if (null != this.scope)
            {
                this.scope.ScopeChanged -= OnScopeChanged;
                this.scope.DefineAdded -= OnDefineAdded;
                this.scope.DefineChanged -= OnDefinedNameChanged;
                this.scope.InheritedComponentAdded -= OnInheritedComponentAdded;
                this.scope.InheritedComponentRemoved -= OnInheritedComponentRemoved;
            }

            this.scope = scope;

            if (null != this.scope)
            {
                this.scope.ScopeChanged += OnScopeChanged;
                this.scope.DefineAdded += OnDefineAdded;
                this.scope.DefineChanged += OnDefinedNameChanged;
                this.scope.InheritedComponentAdded += OnInheritedComponentAdded;
                this.scope.InheritedComponentRemoved += OnInheritedComponentRemoved;
            }

            NotifyScopeChanged();
        }

        public bool DependsOn(ComponentViewModel requiredComponent)
        {
            return Requires.Contains(requiredComponent.Provides);
        }

        public PropertyViewModel GetProperty(string name)
        {
            return Properties.FirstOrDefault(x => x.Name == name);
        }

        public void SetProperty(string name, object value)
        {
            PropertyViewModel property = GetProperty(name);
            if (null != property)
            {
                property.Value = value;
            }
        }

        private void OnDefineAdded(DefineViewModel define)
        {
            if (define.Class == plugin.ClassName)
            {
                NotifyPropertyChanged("Scope");
            }
        }

        private void OnScopeChanged()
        {
            NotifyScopeChanged();
        }

        private void NotifyScopeChanged()
        {
            if (null != ScopeChanged)
            {
                ScopeChanged();
            }

            NotifyPropertyChanged("Scope");
        }

        private void OnDefinedNameChanged(PluginViewModel plugin, string newName)
        {
            if (this.plugin == plugin)
            {
                NotifyPropertyChanged("Scope");
            }
        }

        public bool HasInheritedProperty(string name)
        {
            return null != scope ? scope.HasInheritedComponent(plugin) : false;
        }

        public object GetInheritedValue(string name)
        {
            return null != scope ? scope.GetInheritedValue(plugin, name) : null;
        }

        private void OnInheritedComponentAdded(PluginViewModel plugin)
        {
            if (this.plugin == plugin)
            {
                if (null != InheritedPropertyAdded)
                {
                    foreach (string property in plugin.Properties)
                    {
                        InheritedPropertyAdded(property);
                    }
                }

                NotifyPropertyChanged("Scope");
            }
        }

        private void OnInheritedComponentRemoved(PluginViewModel plugin)
        {
            if (this.plugin == plugin)
            {
                if (null != InheritedPropertyAdded)
                {
                    foreach (string property in plugin.Properties)
                    {
                        InheritedPropertyRemoved(property);
                    }
                }

                NotifyPropertyChanged("Scope");
            }
        }

        private void OnPropertyValueChanged(object sender, PropertyChangedEventArgs args)
        {
            if (null != LocalPropertyChanged && args.PropertyName == "Value")
            {
                PropertyViewModel property = sender as PropertyViewModel;
                LocalPropertyChanged(property.Name);
            }
        }
    }
}
