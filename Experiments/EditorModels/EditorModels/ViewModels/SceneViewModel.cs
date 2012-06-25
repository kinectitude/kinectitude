using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using EditorModels.Base;
using EditorModels.Models;
using System.Collections.Generic;
using EditorModels.ViewModels.Interfaces;
using System;
using System.ComponentModel;

namespace EditorModels.ViewModels
{
    internal sealed class SceneViewModel : BaseViewModel, IEntityNamespace, IAttributeScope, IEntityScope
    {
        private readonly Scene scene;
        private ISceneScope scope;
        private Game game;
        private int nextAttribute;

#if TEST

        public Scene Scene
        {
            get { return scene; }
        }

#endif

        public event DefineAddedEventHandler DefineAdded;
        public event DefinedNameChangedEventHandler DefinedNameChanged;
        public event ScopeChangedEventHandler ScopeChanged;
        public event PluginAddedEventHandler PluginAdded;

        public event KeyEventHandler InheritedAttributeAdded { add { } remove { } }
        public event KeyEventHandler InheritedAttributeRemoved { add { } remove { } }
        public event KeyEventHandler InheritedAttributeChanged { add { } remove { } }

        public string Name
        {
            get { return scene.Name; }
            set
            {
                if (scene.Name != value)
                {
                    scene.Name = value;
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
            get { return Entities.SelectMany(x => x.Plugins).Union(Managers.Select(x => x.Plugin)); }
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
            scene = new Scene();

            Name = name;
            Attributes = new ObservableCollection<AttributeViewModel>();
            Managers = new ObservableCollection<ManagerViewModel>();
            Entities = new ObservableCollection<EntityViewModel>();

            AddAttributeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    AttributeViewModel attribute = new AttributeViewModel(GetNextAttributeKey());
                    AddAttribute(attribute);
                }
            );

            RemoveAttributeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    RemoveAttribute(parameter as AttributeViewModel);
                }
            );

            AddEntityCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    EntityViewModel entity = new EntityViewModel();
                    AddEntity(entity);
                }
            );

            RemoveEntityCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    RemoveEntity(parameter as EntityViewModel);
                }
            );
        }

        public void SetScope(Game game, ISceneScope scope)
        {
            if (null != this.scope)
            {
                this.scope.DefineAdded -= OnDefineAdded;
                this.scope.DefinedNameChanged -= OnDefinedNameChanged;
                this.scope.ScopeChanged -= OnScopeChanged;
            }

            if (null != this.game)
            {
                this.game.RemoveScene(scene);
            }

            this.scope = scope;
            this.game = game;

            if (null != this.game)
            {
                this.game.AddScene(scene);
            }

            if (null != this.scope)
            {
                this.scope.DefineAdded += OnDefineAdded;
                this.scope.DefinedNameChanged += OnDefinedNameChanged;
                this.scope.ScopeChanged += OnScopeChanged;
            }

            RaiseScopeChanged();
        }

        public void AddAttribute(AttributeViewModel attribute)
        {
            attribute.SetScope(scene, this);
            //attribute.PropertyChanged += OnAttributePropertyChanged;
            Attributes.Add(attribute);
        }

        public void RemoveAttribute(AttributeViewModel attribute)
        {
            attribute.SetScope(null, null);
            //attribute.PropertyChanged -= OnAttributePropertyChanged;
            Attributes.Remove(attribute);
        }

        public void AddManager(ManagerViewModel manager)
        {
            manager.SetScene(scene);
            Managers.Add(manager);
        }

        public void RemoveManager(ManagerViewModel manager)
        {
            manager.SetScene(null);
            Managers.Remove(manager);
        }

        public void AddEntity(EntityViewModel entity)
        {
            if (!EntityNameExists(entity.Name))
            {
                entity.SetScope(scene, this);
                Entities.Add(entity);
                entity.Components.CollectionChanged += OnEntityComponentChanged;
                entity.PluginAdded += OnEntityPluginAdded;
            }
        }

        public void RemoveEntity(EntityViewModel entity)
        {
            entity.SetScope(null, null);
            Entities.Remove(entity);
            entity.Components.CollectionChanged -= OnEntityComponentChanged;
            entity.PluginAdded -= OnEntityPluginAdded;
        }

        private string GetNextAttributeKey()
        {
            return string.Format("attribute{0}", nextAttribute++);
        }

        /*private void OnAttributePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Value" && null != AttributeChanged)
            {
                AttributeViewModel attribute = sender as AttributeViewModel;
                AttributeChanged(attribute.Key);
            }
        }*/

        private void OnEntityComponentChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ComponentViewModel component in args.NewItems)
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

        private void RaiseScopeChanged()
        {
            if (null != ScopeChanged)
            {
                ScopeChanged();
            }
        }

        private void OnScopeChanged()
        {

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
            if (null != DefinedNameChanged)
            {
                DefinedNameChanged(plugin, newName);
            }
        }

        public bool EntityNameExists(string name)
        {
            return Entities.Any(x => x.Name == name) || null != scope && scope.EntityNameExists(name);
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
