using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Storage;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kinectitude.Editor.Models
{
    internal sealed class Manager : GameModel<IManagerScope>, IPropertyScope
    {
        private readonly Plugin plugin;

        public string Type
        {
            get { return null != Scope ? Scope.GetDefinedName(plugin) : plugin.ClassName; }
        }

        public Plugin Plugin
        {
            get { return plugin; }
        }

        public string DisplayName
        {
            get { return plugin.Header; }
        }

        public bool IsRequired
        {
            get { return null != Scope ? Scope.RequiresManager(this) : false; }
        }

        public ObservableCollection<Property> Properties { get; private set; }

        public Manager(Plugin plugin)
        {
            if (plugin.Type != PluginType.Manager)
            {
                throw new ArgumentException("Plugin is not a manager");
            }

            this.plugin = plugin;

            Properties = new ObservableCollection<Property>();

            AddDependency<ScopeChanged>("Type");
            AddDependency<ScopeChanged>("IsRequired");
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

        #region IPropertyScope implementation

        public event PropertyEventHandler InheritedPropertyAdded { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyRemoved { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyChanged { add { } remove { } }

        public bool HasInheritedProperty(PluginProperty property)
        {
            return false;
        }

        public object GetInheritedValue(PluginProperty property)
        {
            return property.DefaultValue;
        }

        #endregion
    }
}
