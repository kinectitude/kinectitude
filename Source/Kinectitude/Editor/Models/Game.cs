using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Data.DataContainers;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Transactions;
using Kinectitude.Editor.Models.Values;
using Kinectitude.Editor.Storage;
using Kinectitude.Editor.Views.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Kinectitude.Editor.Models
{
    internal sealed class Game : GameModel<IScope>, IAttributeScope, IEntityScope, ISceneScope
    {
        private string name;
        private int width;
        private int height;
        private bool fullScreen;
        private Scene firstScene;
        private int nextAttribute;
        private int nextScene;
        private int nextPrototype;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    string oldName = name;

                    Workspace.Instance.CommandHistory.Log(
                        "rename game to '" + value + "'",
                        () => Name = value,
                        () => Name = oldName
                    );

                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public int Width
        {
            get { return width; }
            set
            {
                if (width != value)
                {
                    int oldWidth = width;

                    Workspace.Instance.CommandHistory.Log(
                        "set game width",
                        () => Width = value,
                        () => Width = oldWidth
                    );

                    width = value;
                    NotifyPropertyChanged("Width");
                }
            }
        }

        public int Height
        {
            get { return height; }
            set
            {
                if (height != value)
                {
                    int oldHeight = height;

                    Workspace.Instance.CommandHistory.Log(
                        "set game height",
                        () => Height = value,
                        () => Height = oldHeight
                    );

                    height = value;
                    NotifyPropertyChanged("Height");
                }
            }
        }

        public bool IsFullScreen
        {
            get { return fullScreen; }
            set
            {
                if (fullScreen != value)
                {
                    bool oldFullScreen = fullScreen;

                    Workspace.Instance.CommandHistory.Log(
                        "toggle full screen",
                        () => IsFullScreen = value,
                        () => IsFullScreen = oldFullScreen
                    );

                    fullScreen = value;
                    NotifyPropertyChanged("IsFullScreen");
                }
            }
        }

        public Scene FirstScene
        {
            get { return firstScene; }
            set
            {
                if (firstScene != value)
                {
                    Scene oldFirstScene = firstScene;

                    Workspace.Instance.CommandHistory.Log(
                        "set first scene to '" + value.Name + "'",
                        () => FirstScene = value,
                        () => FirstScene = oldFirstScene
                    );

                    firstScene = value;
                    NotifyPropertyChanged("FirstScene");
                }
            }
        }

        public ObservableCollection<Using> Usings { get; private set; }
        public ObservableCollection<Entity> Prototypes { get; private set; }
        public ObservableCollection<Scene> Scenes { get; private set; }
        public ObservableCollection<Attribute> Attributes { get; private set; }

        public ICommand RenameCommand { get; private set; }
        public ICommand AddPrototypeCommand { get; private set; }
        public ICommand AddAttributeCommand { get; private set; }
        public ICommand RemoveAttributeCommand { get; private set; }
        public ICommand AddSceneCommand { get; private set; }
        public ICommand RemoveItemCommand { get; private set; }

        public Game(string name)
        {
            this.name = name;
            
            Usings = new ObservableCollection<Using>();
            Prototypes = new ObservableCollection<Entity>();
            Scenes = new ObservableCollection<Scene>();
            Attributes = new ObservableCollection<Attribute>();

            AddHandler<PluginUsed>(OnPluginUsed);

            RenameCommand = new DelegateCommand(null, (parameter) =>
            {
                DialogService.ShowDialog(DialogService.Constants.NameDialog, this);
            });

            AddPrototypeCommand = new DelegateCommand(null, (parameter) =>
            {
                Entity prototype = CreatePrototype();
                EntityTransaction transaction = new EntityTransaction(Prototypes, prototype);
                    
                DialogService.ShowDialog(DialogService.Constants.EntityDialog, transaction, (result) =>
                {
                    if (result == true)
                    {
                        AddPrototype(prototype);
                    }
                });
            });

            AddSceneCommand = new DelegateCommand(null, (parameter) =>
            {
                Scene scene = CreateScene();

                DialogService.ShowDialog(DialogService.Constants.NameDialog, new SceneTransaction(scene), (result) =>
                {
                    if (result == true)
                    {
                        AddScene(scene);
                    }
                });
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

            RemoveItemCommand = new DelegateCommand(null, (parameter) =>
            {
                Entity entity = parameter as Entity;
                if (null != entity)
                {
                    entity.RemoveFromScope();
                }
                else
                {
                    Scene scene = parameter as Scene;
                    if (null != scene)
                    {
                        RemoveScene(scene);
                    }
                }
            });
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void AddUsing(Using use)
        {
            use.Scope = this;
            Usings.Add(use);

            foreach (var define in use.Defines)
            {
                Notify(new DefineAdded(define));
            }
        }

        public void RemoveUsing(Using use)
        {
            use.Scope = null;
            Usings.Remove(use);

            foreach (var define in use.Defines)
            {
                Notify(new DefineRemoved(define));
            }
        }

        private bool HasPrototypeWithName(string name)
        {
            return Prototypes.Any(x => x.Name == name);
        }

        public Entity CreatePrototype()
        {
            return new Entity() { Name = GetNextPrototypeName() };
        }

        public void AddPrototype(Entity prototype)
        {
            if (null != prototype.Name && !HasPrototypeWithName(prototype.Name))
            {
                prototype.Scope = this;

                foreach (Plugin plugin in prototype.Plugins)
                {
                    DefinePlugin(plugin);
                }

                prototype.IsPrototype = true;
                Prototypes.Add(prototype);

                Workspace.Instance.CommandHistory.Log(
                    "add prototype '" + prototype.Name + "'",
                    () => AddPrototype(prototype),
                    () => RemovePrototype(prototype)
                );
            }
        }

        public void RemovePrototype(Entity prototype)
        {
            prototype.Scope = null;
            prototype.IsPrototype = false;
            Prototypes.Remove(prototype);

            Workspace.Instance.CommandHistory.Log(
                "remove prototype '" + prototype.Name + "'",
                () => RemovePrototype(prototype),
                () => AddPrototype(prototype)
            );
        }

        public Entity GetPrototype(string name)
        {
            return Prototypes.FirstOrDefault(x => x.Name == name);
        }

        public void AddScene(Scene scene)
        {
            scene.Scope = this;

            foreach (Plugin plugin in scene.Plugins)
            {
                DefinePlugin(plugin);
            }

            if (Scenes.Count == 0)
            {
                FirstScene = scene;
            }

            Scenes.Add(scene);

            Workspace.Instance.CommandHistory.Log(
                "add scene '" + scene.Name + "'",
                () => AddScene(scene),
                () => RemoveScene(scene)
            );
        }

        public void RemoveScene(Scene scene)
        {
            scene.Scope = null;
            Scenes.Remove(scene);

            Workspace.Instance.CommandHistory.Log(
                "remove scene '" + scene.Name + "'",
                () => RemoveScene(scene),
                () => AddScene(scene)
            );
        }

        public Scene CreateScene()
        {
            return new Scene(GetNextSceneName());
        }

        public Scene GetScene(string name)
        {
            return Scenes.FirstOrDefault(x => x.Name == name);
        }

        public Attribute GetAttribute(string name)
        {
            return Attributes.FirstOrDefault(x => x.Name == name);
        }

        public void AddAttribute(Attribute attribute)
        {
            attribute.Scope = this;
            Attributes.Add(attribute);

            Workspace.Instance.CommandHistory.Log(
                "add attribute '" + attribute.Name + "'",
                () => AddAttribute(attribute),
                () => RemoveAttribute(attribute)
            );
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

        private string GetNextSceneName()
        {
            string ret = "Scene " + nextScene;

            while (Scenes.Any(x => x.Name == ret))
            {
                nextScene++;
                ret = "Scene " + nextScene;
            }

            return ret;
        }

        private string GetNextPrototypeName()
        {
            string ret = "Prototype" + nextPrototype;

            while (Prototypes.Any(x => x.Name == ret))
            {
                nextPrototype++;
                ret = "Prototype" + nextPrototype;
            }

            return ret;
        }

        private void OnPluginUsed(PluginUsed n)
        {
            DefinePlugin(n.Plugin);
        }

        private bool HasDefineWithName(string name)
        {
            return Usings.Any(x => x.HasDefineWithName(name));
        }

        private bool HasDefineWithClass(string name)
        {
            return Usings.Any(x => x.HasDefineWithClass(name));
        }

        private Using GetUsing(string file)
        {
            return Usings.FirstOrDefault(x => x.File == file);
        }

        private void DefinePlugin(Plugin plugin)
        {
            if (!HasDefineWithClass(plugin.ClassName))
            {
                string name = plugin.ShortName;
                if (HasDefineWithName(name))
                {
                    int sequenceNumber = 0;
                    while (HasDefineWithName(plugin.ShortName + sequenceNumber))
                    {
                        sequenceNumber++;
                    }
                    name = plugin.ShortName + sequenceNumber;
                }

                Using use = GetUsing(plugin.File);
                if (null == use)
                {
                    use = new Using() { File = plugin.File };
                    AddUsing(use);
                }

                Define define = new Define(name, plugin.ClassName);
                use.AddDefine(define);
            }
        }

        public Service GetServiceByType(Plugin type)
        {
            return null;
        }

        #region IEntityScope implementation

        IEnumerable<Entity> IEntityScope.Prototypes
        {
            get { return Prototypes; }
        }

        public bool EntityNameExists(string name)
        {
            return HasPrototypeWithName(name);
        }

        void IEntityScope.RemoveEntity(Entity entity)
        {
            RemovePrototype(entity);
        }

        #endregion

        #region ISceneScope implementation

        IEnumerable<Entity> ISceneScope.Prototypes
        {
            get { return Prototypes; }
        }

        #endregion

        #region IAttributeScope implementation

        Entity IAttributeScope.Entity
        {
            get { return null; }
        }

        Scene IAttributeScope.Scene
        {
            get { return null; }
        }

        Game IAttributeScope.Game
        {
            get { return this; }
        }

        public event AttributeEventHandler InheritedAttributeAdded { add { } remove { } }
        public event AttributeEventHandler InheritedAttributeRemoved { add { } remove { } }
        public event AttributeEventHandler InheritedAttributeChanged { add { } remove { } }

        public Value GetInheritedValue(string key)
        {
            return Attribute.DefaultValue;
        }

        public bool HasInheritedAttribute(string key)
        {
            return false;
        }

        public bool HasLocalAttribute(string key)
        {
            return Attributes.Any(x => x.Name == key);
        }

        #endregion

        #region IPluginNamespace implementation

        public string GetDefinedName(Plugin plugin)
        {
            foreach (Using use in Usings)
            {
                Define define = use.GetDefineByClass(plugin.ClassName);
                if (null != define)
                {
                    return define.Name;
                }
            }

            return plugin.ClassName;
        }

        public Plugin GetPlugin(string name)
        {
            foreach (Using use in Usings)
            {
                Define define = use.GetDefineByName(name);
                if (null != define)
                {
                    name = define.Class;
                    break;
                }
            }

            return Workspace.Instance.GetPlugin(name);
        }

        #endregion
    }
}
