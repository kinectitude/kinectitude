using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Data.DataContainers;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Transactions;
using Kinectitude.Editor.Models.Values;
using Kinectitude.Editor.Storage;
using Kinectitude.Editor.Views.Dialogs;
using Kinectitude.Editor.Views.Scenes.Presenters;
using Kinectitude.Editor.Views.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Kinectitude.Editor.Models
{
    internal sealed class Scene : GameModel<ISceneScope>, IAttributeScope, IEntityScope, IManagerScope
    {
        private string name;
        private int nextAttribute;

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

        public IEnumerable<Plugin> Plugins
        {
            get { return Entities.SelectMany(x => x.Plugins).Union(Managers.Select(x => x.Plugin)).Distinct(); }
        }

        public ObservableCollection<Attribute> Attributes { get; private set; }
        public ObservableCollection<Manager> Managers { get; private set; }
        public ObservableCollection<Entity> Entities { get; private set; }
        public ObservableCollection<EntityPresenter> EntityPresenters { get; private set; }

        public ICommand RenameCommand { get; private set; }
        public ICommand AddAttributeCommand { get; private set; }
        public ICommand RemoveAttributeCommand { get; private set; }
        public ICommand PromptAddEntityCommand { get; private set; }
        public ICommand AddEntityCommand { get; private set; }
        public ICommand RemoveEntityCommand { get; private set; }
        public ICommand PropertiesCommand { get; private set; }

        public ICommand CutCommand { get; private set; }
        public ICommand CopyCommand { get; private set; }
        public ICommand PasteCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public Scene(string name)
        {
            this.name = name;
            
            Attributes = new ObservableCollection<Attribute>();
            Managers = new ObservableCollection<Manager>();
            Entities = new ObservableCollection<Entity>();
            EntityPresenters = new ObservableCollection<EntityPresenter>();

            Entities.CollectionChanged += OnEntitiesChanged;

            RenameCommand = new DelegateCommand(null, (parameter) =>
            {
                DialogService.ShowDialog<NameDialog>(new SceneTransaction(this));
            });

            AddAttributeCommand = new DelegateCommand(null, (parameter) =>
            {
                CreateAttribute();
            });

            RemoveAttributeCommand = new DelegateCommand(null, (parameter) =>
            {
                Attribute attribute = parameter as Attribute;
                RemoveAttribute(attribute);
            });

            AddEntityCommand = new DelegateCommand(null, (parameter) =>
            {
                Entity preset = parameter as Entity;
                    
                if (null != preset)
                {
                    Entity entity = preset.DeepCopy();
                    AddEntity(entity);
                }
            });

            RemoveEntityCommand = new DelegateCommand(null, (parameter) =>
            {
                Entity entity = parameter as Entity;
                RemoveEntity(entity);
            });

            PropertiesCommand = new DelegateCommand(null, (parameter) =>
            {
                DialogService.ShowDialog<SceneDialog>(new SceneTransaction(this));
            });

            CutCommand = new DelegateCommand(null, (parameter) =>
            {
                var selected = (IEnumerable)parameter;
                if (null != selected)
                {
                    var clip = selected.Cast<EntityPresenter>().Select(x => x.Entity).ToArray();

                    foreach (var entity in clip)
                    {
                        RemoveEntity(entity);
                    }

                    Workspace.Instance.ClippedItem = clip;
                }
            });

            CopyCommand = new DelegateCommand(null, (parameter) =>
            {
                var selected = (IEnumerable)parameter;
                if (null != selected)
                {
                    var clip = selected.Cast<EntityPresenter>().Select(x => x.Entity.DeepCopy()).ToArray();
                    Workspace.Instance.ClippedItem = clip;
                }
            });

            PasteCommand = new DelegateCommand(null, (parameter) =>
            {
                var clippedEntities = Workspace.Instance.ClippedItem as IEnumerable<Entity>;
                if (null != clippedEntities)
                {
                    foreach (var entity in clippedEntities)
                    {
                        AddEntity(entity.DeepCopy());
                    }
                }
            });

            DeleteCommand = new DelegateCommand(null, (parameter) =>
            {
                var selected = (IEnumerable)parameter;
                if (null != selected)
                {
                    var entities = selected.Cast<EntityPresenter>().Select(x => x.Entity).ToArray();
                    foreach (var entity in entities)
                    {
                        RemoveEntity(entity);
                    }
                }
            });
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }

        private void OnEntitiesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Entity entity in e.NewItems)
                {
                    EntityPresenters.Add(new EntityPresenter(entity));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Entity entity in e.OldItems)
                {
                    var presenter = EntityPresenters.First(x => x.Entity == entity);
                    EntityPresenters.Remove(presenter);
                }
            }
        }

        public Attribute GetAttribute(string name)
        {
            return Attributes.FirstOrDefault(x => x.Name == name);
        }

        public void AddAttribute(Attribute attribute)
        {
            attribute.Scope = this;
            Attributes.Add(attribute);
        }

        public void RemoveAttribute(Attribute attribute)
        {
            attribute.Scope = null;
            Attributes.Remove(attribute);

            Workspace.Instance.CommandHistory.Log(
                "remove attribute '" + attribute.Name + "'",
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
            manager.Scope = this;
            Managers.Add(manager);

            Notify(new PluginUsed(manager.Plugin));
        }

        public void RemoveManager(Manager manager)
        {
            if (!manager.IsRequired)
            {
                manager.Scope = null;
                Managers.Remove(manager);
            }
        }

        public void AddEntity(Entity entity)
        {
            if (!EntityNameExists(entity.Name))
            {
                entity.Scope = this;
                Entities.Add(entity);

                entity.Components.CollectionChanged += OnEntityComponentChanged;
                foreach (Component component in entity.Components)
                {
                    ResolveComponentDependencies(component);
                }

                foreach (Plugin plugin in entity.Plugins)
                {
                    Notify(new PluginUsed(plugin));
                }

                Workspace.Instance.CommandHistory.Log(
                    "add entity",
                    () => AddEntity(entity),
                    () => RemoveEntity(entity)
                );
            }
        }

        public void RemoveEntity(Entity entity)
        {
            entity.Scope = null;
            Entities.Remove(entity);

            entity.Components.CollectionChanged -= OnEntityComponentChanged;

            Workspace.Instance.CommandHistory.Log(
                "remove entity",
                () => RemoveEntity(entity),
                () => AddEntity(entity)
            );
        }

        public Entity GetEntityByName(string name)
        {
            return Entities.FirstOrDefault(x => x.Name == name);
        }

        private string GetNextAttributeKey()
        {
            string ret = "attribute" + nextAttribute;

            while (Attributes.Any(x => x.Name == ret))
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
                    if (!HasManagerOfType(plugin))
                    {
                        Manager manager = new Manager(plugin);
                        AddManager(manager);
                    }
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

        public Manager GetManagerByType(Plugin type)
        {
            return Managers.FirstOrDefault(x => x.Plugin == type);
        }

        public Manager GetManagerByDefinedName(string name)
        {
            return Managers.FirstOrDefault(x => x.Type == name);
        }

        public bool HasManagerOfType(Plugin type)
        {
            return Managers.Any(x => x.Plugin == type);
        }

        #region IEntityScope implementation

        public IEnumerable<Entity> Prototypes
        {
            get { return null != Scope ? Scope.Prototypes : null; }
        }

        public bool EntityNameExists(string name)
        {
            return Entities.Any(x => x.Name != null && x.Name == name) || null != Scope && Scope.EntityNameExists(name);
        }

        void IEntityScope.RemoveEntity(Entity entity)
        {
            RemoveEntity(entity);
        }

        #endregion

        #region IPluginNamespace implementation

        public Plugin GetPlugin(string type)
        {
            return null != Scope ? Scope.GetPlugin(type) : Workspace.Instance.GetPlugin(type);
        }

        public string GetDefinedName(Plugin plugin)
        {
            return null != Scope ? Scope.GetDefinedName(plugin) : plugin.ClassName;
        }

        #endregion

        #region IAttributeScope implementation

        Entity IAttributeScope.Entity
        {
            get { return null; }
        }

        Scene IAttributeScope.Scene
        {
            get { return this; }
        }

        Game IAttributeScope.Game
        {
            get { return null; }
        }

        public event AttributeEventHandler InheritedAttributeAdded { add { } remove { } }
        public event AttributeEventHandler InheritedAttributeRemoved { add { } remove { } }
        public event AttributeEventHandler InheritedAttributeChanged { add { } remove { } }

        public bool HasInheritedAttribute(string key)
        {
            return false;
        }

        public Value GetInheritedValue(string key)
        {
            return Attribute.DefaultValue;
        }

        public bool HasLocalAttribute(string key)
        {
            return Attributes.Any(x => x.Name == key);
        }

        #endregion

        #region IManagerScope implementation

        public bool RequiresManager(Manager manager)
        {
            foreach (Entity entity in Entities)
            {
                foreach (Component component in entity.Components)
                {
                    if (component.Requires.Contains(manager.Plugin.ClassName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}
