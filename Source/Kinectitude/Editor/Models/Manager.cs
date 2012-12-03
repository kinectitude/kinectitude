using System;
using System.Collections.ObjectModel;
using System.Linq;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Storage;
using Kinectitude.Editor.Models.Notifications;

namespace Kinectitude.Editor.Models
{
    internal sealed class Manager : GameModel<IManagerScope>, IPropertyScope
    {
        private readonly Plugin plugin;

        public event PropertyEventHandler InheritedPropertyAdded { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyRemoved { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyChanged { add { } remove { } }

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
                throw new ArgumentException("Plugin is not an manager");
            }

            this.plugin = plugin;

            Properties = new ObservableCollection<Property>();

            AddDependency<ScopeChanged>("Type");
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
    }
}
