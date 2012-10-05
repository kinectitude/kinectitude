using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;

namespace Kinectitude.Editor.Models
{
    internal abstract class AbstractEvent : BaseModel, IPropertyScope, IActionScope
    {
        private readonly List<AbstractProperty> properties;
        private readonly ObservableCollection<AbstractAction> actions;
        protected IEventScope scope;

        public event ScopeChangedEventHandler ScopeChanged;
        public event PluginAddedEventHandler PluginAdded;
        public event PropertyEventHandler InheritedPropertyAdded { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyRemoved { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyChanged { add { } remove { } }
        public abstract event DefineAddedEventHandler DefineAdded;
        public abstract event DefinedNameChangedEventHandler DefineChanged;

        [DependsOn("Scope")]
        public abstract string Type { get; }

        public abstract string Header { get; }

        public abstract string Description { get; }

        [DependsOn("IsInherited")]
        public abstract bool IsLocal { get; }

        public abstract bool IsInherited { get; }

        public IEnumerable<AbstractProperty> Properties
        {
            get { return properties; }
        }

        public ObservableCollection<AbstractAction> Actions
        {
            get { return actions; }
        }

        public abstract IEnumerable<Plugin> Plugins { get; }

        protected AbstractEvent()
        {
            properties = new List<AbstractProperty>();
            actions = new ObservableCollection<AbstractAction>();
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
            AbstractProperty property = GetProperty(name);
            if (null != property)
            {
                property.Value = value;
            }
        }

        public void AddAction(AbstractAction action)
        {
            action.SetScope(this);
            Actions.Add(action);

            if (null != PluginAdded)
            {
                foreach (Plugin plugin in action.Plugins)
                {
                    PluginAdded(plugin);
                }
            }
        }

        public void RemoveAction(AbstractAction action)
        {
            if (action.IsLocal)
            {
                PrivateRemoveAction(action);
            }
        }

        protected void PrivateRemoveAction(AbstractAction action)
        {
            action.SetScope(null);
            Actions.Remove(action);
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

        bool IPropertyScope.HasInheritedProperty(string name)
        {
            return false;
        }

        object IPropertyScope.GetInheritedValue(string name)
        {
            return null;
        }

        string IPluginNamespace.GetDefinedName(Plugin plugin)
        {
            return null != scope ? scope.GetDefinedName(plugin) : plugin.ClassName;
        }

        Plugin IPluginNamespace.GetPlugin(string name)
        {
            return null != scope ? scope.GetPlugin(name) : Workspace.Instance.GetPlugin(name);
        }

        public abstract bool InheritsFrom(AbstractEvent evt);
    }
}
