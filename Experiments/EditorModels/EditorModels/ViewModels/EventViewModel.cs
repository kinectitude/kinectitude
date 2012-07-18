using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EditorModels.ViewModels.Interfaces;

namespace EditorModels.ViewModels
{
    internal sealed class EventViewModel : BaseViewModel, IPropertyScope
    {
        private readonly PluginViewModel plugin;
        private IEventScope scope;

        public event PluginAddedEventHandler PluginAdded;
        public event ScopeChangedEventHandler ScopeChanged;
        public event PropertyEventHandler InheritedPropertyAdded { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyRemoved { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyChanged { add { } remove { } }

        [DependsOn("Scope")]
        public string Type
        {
            get { return null != scope ? scope.GetDefinedName(plugin) : plugin.ClassName; }
        }

        [DependsOn("IsInherited")]
        public bool IsLocal
        {
            get { return !IsInherited; }
        }

        public bool IsInherited
        {
            get { return false; }
        }

        public ObservableCollection<PropertyViewModel> Properties
        {
            get;
            private set;
        }

        public ObservableCollection<ActionViewModel> Actions
        {
            get;
            private set;
        }

        public IEnumerable<PluginViewModel> Plugins
        {
            get { return Actions.SelectMany(x => x.Plugins).Union(Enumerable.Repeat(plugin, 1)); }
        }

        public EventViewModel(PluginViewModel plugin)
        {
            this.plugin = plugin;

            Properties = new ObservableCollection<PropertyViewModel>();
            Actions = new ObservableCollection<ActionViewModel>();
        }

        public void AddAction(ActionViewModel action)
        {
            // TODO: Notify plugin added
        }

        private void NotifyPluginAdded(PluginViewModel plugin)
        {
            if (null != PluginAdded)
            {
                PluginAdded(plugin);
            }
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

        public void SetScope(IEventScope scope)
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
