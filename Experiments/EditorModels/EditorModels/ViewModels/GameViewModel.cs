using System.Collections.ObjectModel;
using System.Linq;
using EditorModels.Base;
using EditorModels.Models;
using EditorModels.Storage;
using EditorModels.ViewModels.Interfaces;
using System;
using System.ComponentModel;

namespace EditorModels.ViewModels
{
    internal delegate void ScopeChangedEventHandler();
    internal delegate void DefineAddedEventHandler(DefineViewModel define);
    internal delegate void PluginAddedEventHandler(PluginViewModel plugin);
    internal delegate void DefinedNameChangedEventHandler(PluginViewModel plugin, string newName);
    
    internal sealed class GameViewModel : BaseViewModel, IAttributeScope, IEntityScope, ISceneScope
    {
        private readonly Game game;
        private SceneViewModel firstScene;
        private int nextAttribute;
        private int nextScene;

#if TEST

        public Game Game
        {
            get { return game; }
        }

#endif

        public event ScopeChangedEventHandler ScopeChanged { add { } remove { } }

        public event DefineAddedEventHandler DefineAdded;
        public event DefinedNameChangedEventHandler DefinedNameChanged;

        public event KeyEventHandler InheritedAttributeAdded { add { } remove { } }
        public event KeyEventHandler InheritedAttributeRemoved { add { } remove { } }
        public event KeyEventHandler InheritedAttributeChanged { add { } remove { } }

        public string FileName
        {
            get;
            set;
        }

        public string Name
        {
            get { return game.Name; }
            set
            {
                if (game.Name != value)
                {
                    game.Name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public int Width
        {
            get { return game.Width; }
            set
            {
                if (game.Width != value)
                {
                    game.Width = value;
                    NotifyPropertyChanged("Width");
                }
            }
        }

        public int Height
        {
            get { return game.Height; }
            set
            {
                if (game.Height != value)
                {
                    game.Height = value;
                    NotifyPropertyChanged("Height");
                }
            }
        }

        public bool IsFullScreen
        {
            get { return game.IsFullScreen; }
            set
            {
                if (game.IsFullScreen != value)
                {
                    game.IsFullScreen = value;
                    NotifyPropertyChanged("IsFullScreen");
                }
            }
        }

        public SceneViewModel FirstScene
        {
            get { return firstScene; }
            set
            {
                if (firstScene != value)
                {
                    firstScene = value;
                    game.FirstScene = firstScene.Name;
                    NotifyPropertyChanged("FirstScene");
                }
            }
        }

        public ObservableCollection<UsingViewModel> Usings
        {
            get;
            private set;
        }

        public ObservableCollection<EntityViewModel> Prototypes
        {
            get;
            private set;
        }

        public ObservableCollection<SceneViewModel> Scenes
        {
            get;
            private set;
        }

        public ObservableCollection<AttributeViewModel> Attributes
        {
            get;
            private set;
        }

        public ObservableCollection<AssetViewModel> Assets
        {
            get;
            private set;
        }

        public BaseViewModel CurrentItem
        {
            get;
            set;
        }

        public ICommand AddPrototypeCommand
        {
            get;
            private set;
        }

        public ICommand RemovePrototypeCommand
        {
            get;
            private set;
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

        public ICommand AddSceneCommand
        {
            get;
            private set;
        }

        public ICommand RemoveSceneCommand
        {
            get;
            private set;
        }

        public ICommand AddAssetCommand
        {
            get;
            private set;
        }

        public ICommand RemoveAssetCommand
        {
            get;
            private set;
        }

        public ICommand SaveGameCommand
        {
            get;
            private set;
        }

        public GameViewModel(string name)
        {
            game = new Game();

            Name = name;
            Usings = new ObservableCollection<UsingViewModel>();
            Assets = new ObservableCollection<AssetViewModel>();
            Prototypes = new ObservableCollection<EntityViewModel>();
            Scenes = new ObservableCollection<SceneViewModel>();
            Attributes = new ObservableCollection<AttributeViewModel>();

            AddPrototypeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    EntityViewModel prototype = new EntityViewModel();
                    // TODO Create UI to fill in prototype
                    AddPrototype(prototype);
                }
            );

            RemovePrototypeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    RemovePrototype(parameter as EntityViewModel);
                }
            );

            AddSceneCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    SceneViewModel scene = new SceneViewModel(GetNextSceneName());
                    AddScene(scene);
                }
            );

            RemoveSceneCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    RemoveScene(parameter as SceneViewModel);
                }
            );

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

            AddAssetCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    // TODO: File Chooser
                    AssetViewModel asset = new AssetViewModel("An Asset");
                    AddAsset(asset);
                }
            );

            RemoveAssetCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    RemoveAsset(parameter as AssetViewModel);
                }
            );

            SaveGameCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    SaveGame(parameter as string);  // TODO: remove hard-coded string
                }
            );
        }

        public void AddUsing(UsingViewModel use)
        {
            use.SetGame(game);
            use.DefineAdded += OnUsingAddedDefine;
            use.DefineChanged += OnDefineChanged;
            Usings.Add(use);
        }

        public void RemoveUsing(UsingViewModel use)
        {
            use.SetGame(null);
            use.DefineAdded -= OnUsingAddedDefine;
            use.DefineChanged -= OnDefineChanged;
            Usings.Remove(use);
        }

        public void AddPrototype(EntityViewModel prototype)
        {
            prototype.SetScope(game, this);
            
            prototype.PluginAdded += OnPluginAdded;
            foreach (PluginViewModel plugin in prototype.Plugins)
            {
                DefinePlugin(plugin);
            }

            Prototypes.Add(prototype);
        }

        public void RemovePrototype(EntityViewModel prototype)
        {
            prototype.SetScope(null, null);
            prototype.PluginAdded -= OnPluginAdded;
            Prototypes.Remove(prototype);
        }

        public EntityViewModel GetPrototype(string name)
        {
            return Prototypes.FirstOrDefault(x => x.Name == name);
        }

        public void AddScene(SceneViewModel scene)
        {
            scene.SetScope(game, this);

            scene.PluginAdded += OnPluginAdded;
            foreach (PluginViewModel plugin in scene.Plugins)
            {
                DefinePlugin(plugin);
            }
            
            Scenes.Add(scene);
        }

        public void RemoveScene(SceneViewModel scene)
        {
            scene.SetScope(null, null);
            scene.PluginAdded -= OnPluginAdded;
            Scenes.Remove(scene);
        }

        public SceneViewModel GetScene(string name)
        {
            return Scenes.FirstOrDefault(x => x.Name == name);
        }

        public void AddAttribute(AttributeViewModel attribute)
        {
            attribute.SetScope(game, this);
            Attributes.Add(attribute);
        }

        public void RemoveAttribute(AttributeViewModel attribute)
        {
            attribute.SetScope(null, null);
            Attributes.Remove(attribute);
        }

        public void AddAsset(AssetViewModel asset)
        {
            asset.SetGame(game);
            Assets.Add(asset);
        }

        public void RemoveAsset(AssetViewModel asset)
        {
            asset.SetGame(null);
            Assets.Remove(asset);
        }

        public void SaveGame(string file)
        {
            //IGameStorage storage = new XmlGameStorage(FileName);
            IGameStorage storage = new XmlGameStorage(file);
            storage.SaveGame(game);
        }

        public PluginViewModel GetPlugin(string name)
        {
            foreach (UsingViewModel use in Usings)
            {
                DefineViewModel define = use.GetDefineByName(name);
                if (null != define)
                {
                    name = define.Class;
                    break;
                }
            }

            return Workspace.Instance.GetPlugin(name);
        }

        private string GetNextAttributeKey()
        {
            return string.Format("attribute{0}", nextAttribute++);
        }

        private string GetNextSceneName()
        {
            return string.Format("Scene {0}", nextScene++);
        }

        private void OnPluginAdded(PluginViewModel plugin)
        {
            DefinePlugin(plugin);
        }

        private bool HasDefineWithName(string name)
        {
            return Usings.Any(x => x.HasDefineWithName(name));
        }

        private bool HasDefineWithClass(string name)
        {
            return Usings.Any(x => x.HasDefineWithClass(name));
        }

        private UsingViewModel GetUsing(string file)
        {
            return Usings.FirstOrDefault(x => x.File == file);
        }

        private void DefinePlugin(PluginViewModel plugin)
        {
            if (!HasDefineWithClass(plugin.ClassName))
            {
                string name = plugin.ShortName;
                if (HasDefineWithName(name))
                {
                    int sequenceNumber = 0;
                    while (HasDefineWithName(string.Format("{0}{1}", plugin.ShortName, sequenceNumber)))
                    {
                        sequenceNumber++;
                    }
                    name = string.Format("{0}{1}", plugin.ShortName, sequenceNumber);
                }

                UsingViewModel use = GetUsing(plugin.File);
                if (null == use)
                {
                    use = new UsingViewModel() { File = plugin.File };
                    AddUsing(use);
                }

                DefineViewModel define = new DefineViewModel(name, plugin.ClassName);
                use.AddDefine(define);
            }
        }

        private void OnUsingAddedDefine(DefineViewModel define)
        {
            if (null != DefineAdded)
            {
                DefineAdded(define);
            }
        }

        private void OnDefineChanged(DefineViewModel define)
        {
            if (null != DefinedNameChanged)
            {
                PluginViewModel plugin = GetPlugin(define.Class);
                DefinedNameChanged(plugin, define.Name);
            }
        }

        bool IEntityNamespace.EntityNameExists(string name)
        {
            return Prototypes.Any(x => x.Name == name);
        }

        string IAttributeScope.GetInheritedValue(string key)
        {
            return null;
        }

        bool IAttributeScope.HasInheritedAttribute(string key)
        {
            return false;
        }

        bool IAttributeScope.HasLocalAttribute(string key)
        {
            return Attributes.Any(x => x.Key == key);
        }

        string IPluginNamespace.GetDefinedName(PluginViewModel plugin)
        {
            foreach (UsingViewModel use in Usings)
            {
                DefineViewModel define = use.GetDefineByClass(plugin.ClassName);
                if (null != define)
                {
                    return define.Name;
                }
            }

            return null;
        }
    }
}
