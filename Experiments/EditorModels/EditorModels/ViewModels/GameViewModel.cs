using System.Collections.ObjectModel;
using System.Linq;
using EditorModels.Base;
using EditorModels.Storage;
using EditorModels.ViewModels.Interfaces;

namespace EditorModels.ViewModels
{
    internal delegate void DefineAddedEventHandler(DefineViewModel define);
    internal delegate void PluginAddedEventHandler(PluginViewModel plugin);
    internal delegate void DefinedNameChangedEventHandler(PluginViewModel plugin, string newName);
    
    internal sealed class GameViewModel : BaseViewModel, IAttributeScope, IEntityScope, ISceneScope
    {
        private string name;
        private int width;
        private int height;
        private bool fullScreen;
        private SceneViewModel firstScene;
        private int nextAttribute;
        private int nextScene;

        public event ScopeChangedEventHandler ScopeChanged { add { } remove { } }

        public event DefineAddedEventHandler DefineAdded;
        public event DefinedNameChangedEventHandler DefineChanged;

        public event AttributeEventHandler InheritedAttributeAdded { add { } remove { } }
        public event AttributeEventHandler InheritedAttributeRemoved { add { } remove { } }
        public event AttributeEventHandler InheritedAttributeChanged { add { } remove { } }

        public string FileName
        {
            get;
            set;
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
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
                    fullScreen = value;
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
            this.name = name;
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
                    SaveGame();  // TODO: remove hard-coded string
                }
            );
        }

        public void AddUsing(UsingViewModel use)
        {
            use.DefineAdded += OnDefineAdded;
            use.DefineChanged += OnDefineChanged;
            Usings.Add(use);
        }

        public void RemoveUsing(UsingViewModel use)
        {
            use.DefineAdded -= OnDefineAdded;
            use.DefineChanged -= OnDefineChanged;
            Usings.Remove(use);
        }

        private bool HasPrototypeWithName(string name)
        {
            return Prototypes.Any(x => x.Name == name);
        }

        public void AddPrototype(EntityViewModel prototype)
        {
            if (null != prototype.Name && !HasPrototypeWithName(prototype.Name))
            {
                prototype.SetScope(this);

                prototype.PluginAdded += OnPluginAdded;
                foreach (PluginViewModel plugin in prototype.Plugins)
                {
                    DefinePlugin(plugin);
                }

                Prototypes.Add(prototype);
            }
        }

        public void RemovePrototype(EntityViewModel prototype)
        {
            prototype.SetScope(null);
            prototype.PluginAdded -= OnPluginAdded;
            Prototypes.Remove(prototype);
        }

        public EntityViewModel GetPrototype(string name)
        {
            return Prototypes.FirstOrDefault(x => x.Name == name);
        }

        public void AddScene(SceneViewModel scene)
        {
            scene.SetScope(this);

            scene.PluginAdded += OnPluginAdded;
            foreach (PluginViewModel plugin in scene.Plugins)
            {
                DefinePlugin(plugin);
            }
            
            Scenes.Add(scene);
        }

        public void RemoveScene(SceneViewModel scene)
        {
            scene.SetScope(null);
            scene.PluginAdded -= OnPluginAdded;
            Scenes.Remove(scene);
        }

        public SceneViewModel GetScene(string name)
        {
            return Scenes.FirstOrDefault(x => x.Name == name);
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

        public void AddAsset(AssetViewModel asset)
        {
            Assets.Add(asset);
        }

        public void RemoveAsset(AssetViewModel asset)
        {
            Assets.Remove(asset);
        }

        public void SaveGame()
        {
            IGameStorage storage = new XmlGameStorage(FileName);
            storage.SaveGame(this);
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

        private void OnDefineAdded(DefineViewModel define)
        {
            if (null != DefineAdded)
            {
                DefineAdded(define);
            }
        }

        private void OnDefineChanged(DefineViewModel define)
        {
            if (null != DefineChanged)
            {
                PluginViewModel plugin = GetPlugin(define.Class);
                DefineChanged(plugin, define.Name);
            }
        }

        bool IEntityNamespace.EntityNameExists(string name)
        {
            return HasPrototypeWithName(name);
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
