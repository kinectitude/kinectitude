using System;
using System.Collections.ObjectModel;
using System.Linq;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models
{
    internal sealed class Manager : VisitableModel, IPropertyScope
    {
        private readonly Plugin plugin;
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

        public Plugin Plugin
        {
            get { return plugin; }
        }

        public ObservableCollection<Property> Properties
        {
            get;
            private set;
        }

        public Manager(Plugin plugin)
        {
            if (plugin.Type != PluginType.Manager)
            {
                throw new ArgumentException("Plugin is not an manager");
            }

            this.plugin = plugin;

            Properties = new ObservableCollection<Property>();
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }

        public Property GetProperty(string name)
        {
            return Properties.FirstOrDefault(x => x.Name == name);
        }

        public void SetProperty(string name, object value)
        {
            Property property = GetProperty(name);
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
