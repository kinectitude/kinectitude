using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using System.Windows.Input;
using Kinectitude.Editor.Presenters;
using System;
using Kinectitude.Editor.Views;

namespace Kinectitude.Editor.Models
{
    internal sealed class Scene : BaseModel, IEntityNamespace, IAttributeScope, IEntityScope, IManagerScope
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

        public ObservableCollection<Attribute> Attributes
        {
            get;
            private set;
        }

        public ObservableCollection<Manager> Managers
        {
            get;
            private set;
        }

        public ObservableCollection<Entity> Entities
        {
            get;
            private set;
        }

        public ObservableCollection<Entity> SelectedEntities
        {
            get;
            private set;
        }

        public IEnumerable<Plugin> Plugins
        {
            get { return Entities.SelectMany(x => x.Plugins).Union(Managers.Select(x => x.Plugin)).Distinct(); }
        }

        public ICommand SelectCommand { get; private set; }
        public ICommand AddAttributeCommand { get; private set; }
        public ICommand RemoveAttributeCommand { get; private set; }
        public ICommand PromptAddEntityCommand { get; private set; }
        public ICommand AddEntityCommand { get; private set; }
        public ICommand RemoveEntityCommand { get; private set; }

        public Scene(string name)
        {
            this.name = name;
            
            Attributes = new ObservableCollection<Attribute>();
            Managers = new ObservableCollection<Manager>();
            Entities = new ObservableCollection<Entity>();
            SelectedEntities = new ObservableCollection<Entity>();

            AddAttributeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    CreateAttribute();
                }
            );

            RemoveAttributeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Attribute attribute = parameter as Attribute;
                    RemoveAttribute(attribute);
                }
            );

            AddEntityCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    EntityPreset preset = parameter as EntityPreset;
                    
                    if (null != preset)
                    {
                        Entity entity = new Entity();

                        foreach (Plugin plugin in preset.Plugins)
                        {
                            Component component = new Component(plugin);
                            entity.AddComponent(component);
                        }

                        AddEntity(entity);
                        SelectEntity(entity);
                    }
                }
            );

            RemoveEntityCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Entity entity = parameter as Entity;
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

        public void AddAttribute(Attribute attribute)
        {
            attribute.SetScope(this);
            Attributes.Add(attribute);
        }

        public void RemoveAttribute(Attribute attribute)
        {
            attribute.SetScope(null);
            Attributes.Remove(attribute);

            Workspace.Instance.CommandHistory.Log(
                "remove attribute '" + attribute.Key + "'",
                () => RemoveAttribute(attribute),
                () => AddAttribute(attribute)
            );
        }

        public void CreateAttribute()
        {
            Attribute attribute = new Attribute(GetNextAttributeKey());
            AddAttribute(attribute);
        }

        public void AddManager(Manager manager)
        {
            manager.SetScope(this);
            Managers.Add(manager);
        }

        public void RemoveManager(Manager manager)
        {
            manager.SetScope(null);
            Managers.Remove(manager);
        }

        public void AddEntity(Entity entity)
        {
            if (!EntityNameExists(entity.Name))
            {
                entity.SetScope(this);

                Entities.Add(entity);

                entity.Components.CollectionChanged += OnEntityComponentChanged;
                foreach (Component component in entity.Components)
                {
                    ResolveComponentDependencies(component);
                }

                entity.PluginAdded += OnEntityPluginAdded;

                Workspace.Instance.CommandHistory.Log(
                    "add entity",
                    () => AddEntity(entity),
                    () => RemoveEntity(entity)
                );
            }
        }

        public void RemoveEntity(Entity entity)
        {
            entity.SetScope(null);
            Entities.Remove(entity);
            entity.Components.CollectionChanged -= OnEntityComponentChanged;
            entity.PluginAdded -= OnEntityPluginAdded;

            Workspace.Instance.CommandHistory.Log(
                "remove entity",
                () => RemoveEntity(entity),
                () => AddEntity(entity)
            );
        }

        public void SelectEntity(Entity entity)
        {
            DeselectAll();
            SelectedEntities.Add(entity);
        }

        public void DeselectAll()
        {
            SelectedEntities.Clear();
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

        private void ResolveComponentDependencies(Component component)
        {
            foreach (string require in component.Requires)
            {
                Plugin plugin = GetPlugin(require);
                if (plugin.Type == PluginType.Manager)
                {
                    Manager manager = new Manager(plugin);
                    AddManager(manager);
                }
            }
        }

        private void OnEntityComponentChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Component component in args.NewItems)
                {
                    ResolveComponentDependencies(component);
                }
            }
        }

        public Plugin GetPlugin(string type)
        {
            return null != scope ? scope.GetPlugin(type) : Workspace.Instance.GetPlugin(type);
        }

        private void OnEntityPluginAdded(Plugin plugin)
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

        private void OnDefineAdded(Define define)
        {
            if (null != DefineAdded)
            {
                DefineAdded(define);
            }
        }

        private void OnDefinedNameChanged(Plugin plugin, string newName)
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

        string IPluginNamespace.GetDefinedName(Plugin plugin)
        {
            return null != scope ? scope.GetDefinedName(plugin) : plugin.ClassName;
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
