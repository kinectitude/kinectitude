using System.Collections.ObjectModel;
using System.Linq;
using Kinectitude.Editor.ViewModels.Interfaces;

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class ManagerViewModel : BaseViewModel, IPropertyScope
    {
        private readonly PluginViewModel plugin;
        private IManagerScope scope;

        public event ScopeChangedEventHandler ScopeChanged;
        public event PropertyEventHandler InheritedPropertyAdded { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyRemoved { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyChanged { add { } remove { } }

        [DependsOn("Scope")]
        public string Type
        {
            get { return null != scope ? scope.GetDefinedName(plugin) : plugin.ClassName; }
        }

        public PluginViewModel Plugin
        {
            get { return plugin; }
        }

        public ObservableCollection<PropertyViewModel> Properties
        {
            get;
            private set;
        }

        public ManagerViewModel(PluginViewModel plugin)
        {
            this.plugin = plugin;

            Properties = new ObservableCollection<PropertyViewModel>();
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

        bool IPropertyScope.HasInheritedProperty(string name)
        {
            return false;
        }

        object IPropertyScope.GetInheritedValue(string name)
        {
            return null;
        }

        public void SetScope(IManagerScope scope)
        {
            if (null != this.scope)
            {
                this.scope.ScopeChanged -= OnScopeChanged;
            }

            this.scope = scope;

            if (null != this.scope)
            {
                this.scope.ScopeChanged += OnScopeChanged;
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
    }
}
