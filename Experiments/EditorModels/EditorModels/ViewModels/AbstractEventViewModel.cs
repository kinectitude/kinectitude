using System.Collections.Generic;
using EditorModels.ViewModels.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;

namespace EditorModels.ViewModels
{
    internal abstract class AbstractEventViewModel : BaseViewModel, IPropertyScope, IActionScope
    {
        private readonly List<AbstractPropertyViewModel> properties;
        private readonly ObservableCollection<AbstractActionViewModel> actions;
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

        [DependsOn("IsInherited")]
        public abstract bool IsLocal { get; }

        public abstract bool IsInherited { get; }

        public IEnumerable<AbstractPropertyViewModel> Properties
        {
            get { return properties; }
        }

        public ObservableCollection<AbstractActionViewModel> Actions
        {
            get { return actions; }
        }

        public abstract IEnumerable<PluginViewModel> Plugins { get; }

        protected AbstractEventViewModel()
        {
            properties = new List<AbstractPropertyViewModel>();
            actions = new ObservableCollection<AbstractActionViewModel>();
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
            AbstractPropertyViewModel property = GetProperty(name);
            if (null != property)
            {
                property.Value = value;
            }
        }

        public void AddAction(AbstractActionViewModel action)
        {
            action.SetScope(this);
            Actions.Add(action);

            if (null != PluginAdded)
            {
                foreach (PluginViewModel plugin in action.Plugins)
                {
                    PluginAdded(plugin);
                }
            }
        }

        public void RemoveAction(AbstractActionViewModel action)
        {
            if (action.IsLocal)
            {
                PrivateRemoveAction(action);
            }
        }

        protected void PrivateRemoveAction(AbstractActionViewModel action)
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

        string IPluginNamespace.GetDefinedName(PluginViewModel plugin)
        {
            return null != scope ? scope.GetDefinedName(plugin) : plugin.ClassName;
        }

        PluginViewModel IPluginNamespace.GetPlugin(string name)
        {
            return null != scope ? scope.GetPlugin(name) : Workspace.Instance.GetPlugin(name);
        }

        public abstract bool InheritsFrom(AbstractEventViewModel evt);
    }
}
