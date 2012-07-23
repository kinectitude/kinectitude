using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.ViewModels.Interfaces;
using System.Windows.Input;

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class SceneViewModel : BaseViewModel, IEntityNamespace, IAttributeScope, IEntityScope, IManagerScope
    {
        private string name;
        private ISceneScope scope;
        private int nextAttribute;

        public event DefineAddedEventHandler DefineAdded;
        public event DefinedNameChangedEventHandler DefineChanged;
        public event ScopeChangedEventHandler ScopeChanged;
        public event PluginAddedEventHandler PluginAdded;

        public event AttributeEventHandler InheritedAttributeAdded { add { } remove { } }
        public event AttributeEventHandler InheritedAttributeRemoved { add { } remove { } }
        public event AttributeEventHandler InheritedAttributeChanged { add { } remove { } }

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    string oldName = name;

                    Workspace.Instance.CommandHistory.Log(
                        "rename scene to '" + value + "'",
                        () => Name = value,
                        () => Name = oldName
                    );

                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public ObservableCollection<AttributeViewModel> Attributes
        {
            get;
            private set;
        }

        public ObservableCollection<ManagerViewModel> Managers
        {
            get;
            private set;
        }

        public ObservableCollection<EntityViewModel> Entities
        {
            get;
            private set;
        }

        public IEnumerable<PluginViewModel> Plugins
        {
            get { return Entities.SelectMany(x => x.Plugins).Union(Managers.Select(x => x.Plugin)).Distinct(); }
        }

        public ICommand AddAttributeCommand
        {
            get;
            private set;
        }

        public ICommand RemoveAttributeCommand
        {
            get;
            private set;
        }

        public ICommand AddEntityCommand
        {
            get;
            private set;
        }

        public ICommand RemoveEntityCommand
        {
            get;
            private set;
        }

        public SceneViewModel(string name)
        {
            this.name = name;
            Attributes = new ObservableCollection<AttributeViewModel>();
            Managers = new ObservableCollection<ManagerViewModel>();
            Entities = new ObservableCollection<EntityViewModel>();

            AddAttributeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    AttributeViewModel attribute = new AttributeViewModel(GetNextAttributeKey());

                    Workspace.Instance.CommandHistory.Log(
                        "add attribute '" + attribute.Key + "'",
                        () => AddAttribute(attribute),
                        () => RemoveAttribute(attribute)
                    );

                    AddAttribute(attribute);
                }
            );

            RemoveAttributeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    AttributeViewModel attribute = parameter as AttributeViewModel;

                    Workspace.Instance.CommandHistory.Log(
                        "remove attribute '" + attribute.Key + "'",
                        () => RemoveAttribute(attribute),
                        () => AddAttribute(attribute)
                    );

                    RemoveAttribute(attribute);
                }
            );

            AddEntityCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    EntityViewModel entity = new EntityViewModel();

                    Workspace.Instance.CommandHistory.Log(
                        "add entity",
                        () => AddEntity(entity),
                        () => RemoveEntity(entity)
                    );

                    AddEntity(entity);
                }
            );

            RemoveEntityCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    EntityViewModel entity = parameter as EntityViewModel;

                    Workspace.Instance.CommandHistory.Log(
                        "remove entity",
                        () => RemoveEntity(entity),
                        () => AddEntity(entity)
                    );

                    RemoveEntity(entity);
                }
            );
        }

        public void SetScope(ISceneScope scope)
        {
            if (null != this.scope)
            {
                this.scope.DefineAdded -= OnDefineAdded;
                this.scope.DefineChanged -= OnDefinedNameChanged;
                this.scope.ScopeChanged -= OnScopeChanged;
            }

            this.scope = scope;

            if (null != this.scope)
            {
                this.scope.DefineAdded += OnDefineAdded;
                this.scope.DefineChanged += OnDefinedNameChanged;
                this.scope.ScopeChanged += OnScopeChanged;
            }

            NotifyScopeChanged();
        }

        public void AddAttribute(AttributeViewModel attribute)
        {
            attribute.SetScope(this);
            Attributes.Add(attribute);
        }

        public void RemoveAttribute(AttributeViewModel attribute)
        {
            attribute.SetScope(null);
            Attributes.Remove(attribute);
        }

        public void AddManager(ManagerViewModel manager)
        {
            manager.SetScope(this);
            Managers.Add(manager);
        }

        public void RemoveManager(ManagerViewModel manager)
        {
            manager.SetScope(null);
            Managers.Remove(manager);
        }

        public void AddEntity(EntityViewModel entity)
        {
            if (!EntityNameExists(entity.Name))
            {
                entity.SetScope(this);
                Entities.Add(entity);

                entity.Components.CollectionChanged += OnEntityComponentChanged;
                foreach (ComponentViewModel component in entity.Components)
                {
                    ResolveComponentDependencies(component);
                }

                entity.PluginAdded += OnEntityPluginAdded;
            }
        }

        public void RemoveEntity(EntityViewModel entity)
        {
            entity.SetScope(null);
            Entities.Remove(entity);
            entity.Components.CollectionChanged -= OnEntityComponentChanged;
            entity.PluginAdded -= OnEntityPluginAdded;
        }

        private string GetNextAttributeKey()
        {
            string ret = "attribute" + nextAttribute;

            while (Attributes.Any(x => x.Key == ret))
            {
                nextAttribute++;
                ret = "attribute" + nextAttribute;
            }

            return ret;
        }

        private void ResolveComponentDependencies(ComponentViewModel component)
        {
            foreach (string require in component.Requires)
            {
                PluginViewModel plugin = GetPlugin(require);
                if (plugin.Type == PluginType.Manager)
                {
                    ManagerViewModel manager = new ManagerViewModel(plugin);
                    AddManager(manager);
                }
            }
        }

        private void OnEntityComponentChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ComponentViewModel component in args.NewItems)
                {
                    ResolveComponentDependencies(component);
                }
            }
        }

        public PluginViewModel GetPlugin(string type)
        {
            return null != scope ? scope.GetPlugin(type) : Workspace.Instance.GetPlugin(type);
        }

        private void OnEntityPluginAdded(PluginViewModel plugin)
        {
            if (null != PluginAdded)
            {
                PluginAdded(plugin);
            }
        }

        private void NotifyScopeChanged()
        {
            if (null != ScopeChanged)
            {
                ScopeChanged();
            }

            NotifyPropertyChanged("Scope");
        }

        private void OnScopeChanged()
        {
            NotifyScopeChanged();
        }

        private void OnDefineAdded(DefineViewModel define)
        {
            if (null != DefineAdded)
            {
                DefineAdded(define);
            }
        }

        private void OnDefinedNameChanged(PluginViewModel plugin, string newName)
        {
            if (null != DefineChanged)
            {
                DefineChanged(plugin, newName);
            }
        }

        public bool EntityNameExists(string name)
        {
            return Entities.Any(x => x.Name != null && x.Name == name) || null != scope && scope.EntityNameExists(name);
        }

        string IPluginNamespace.GetDefinedName(PluginViewModel plugin)
        {
            return null != scope ? scope.GetDefinedName(plugin) : null;
        }

        bool IAttributeScope.HasInheritedAttribute(string key)
        {
            return false;
        }

        string IAttributeScope.GetInheritedValue(string key)
        {
            return null;
        }

        bool IAttributeScope.HasLocalAttribute(string key)
        {
            return Attributes.Any(x => x.Key == key);
        }
    }
}
