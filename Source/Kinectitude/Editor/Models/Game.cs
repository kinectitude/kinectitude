using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Views;

namespace Kinectitude.Editor.Models
{
    internal delegate void PluginAddedEventHandler(Plugin plugin);   
    
    internal sealed class Game : BaseModel, IAttributeScope, IEntityScope, ISceneScope
    {
        private string fileName;
        private string name;
        private int width;
        private int height;
        private bool fullScreen;
        private Scene firstScene;
        private int nextAttribute;
        private int nextScene;
        private int nextPrototype;

        public event ScopeChangedEventHandler ScopeChanged { add { } remove { } }

        public event DefineAddedEventHandler DefineAdded;
        public event DefinedNameChangedEventHandler DefineChanged;

        public event AttributeEventHandler InheritedAttributeAdded { add { } remove { } }
        public event AttributeEventHandler InheritedAttributeRemoved { add { } remove { } }
        public event AttributeEventHandler InheritedAttributeChanged { add { } remove { } }

        public string FileName
        {
            get { return fileName; }
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    NotifyPropertyChanged("FileName");
                }
            }
        }

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

        public ObservableCollection<Using> Usings
        {
            get;
            private set;
        }

        public ObservableCollection<Entity> Prototypes
        {
            get;
            private set;
        }

        public ObservableCollection<Scene> Scenes
        {
            get;
            private set;
        }

        public ObservableCollection<Attribute> Attributes
        {
            get;
            private set;
        }

        public ObservableCollection<Asset> Assets
        {
            get;
            private set;
        }

        public ICommand RenameCommand { get; private set; }
        public ICommand AddPrototypeCommand { get; private set; }
        public ICommand RemovePrototypeCommand { get; private set; }
        public ICommand AddAttributeCommand { get; private set; }
        public ICommand RemoveAttributeCommand { get; private set; }
        public ICommand AddSceneCommand { get; private set; }
        public ICommand RemoveSceneCommand { get; private set; }
        public ICommand AddAssetCommand { get; private set; }
        public ICommand RemoveAssetCommand { get; private set; }
        public ICommand RemoveItemCommand { get; private set; }

        public Game(string name)
        {
            this.name = name;
            
            Usings = new ObservableCollection<Using>();
            Assets = new ObservableCollection<Asset>();
            Prototypes = new ObservableCollection<Entity>();
            Scenes = new ObservableCollection<Scene>();
            Attributes = new ObservableCollection<Attribute>();

            RenameCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    DialogService.ShowDialog(DialogService.Constants.RenameDialog, this);
                }
            );

            AddPrototypeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Entity prototype = CreatePrototype();

                    DialogService.ShowDialog(DialogService.Constants.EntityDialog, prototype,
                        (result) =>
                        {
                            if (result == true)
                            {
                                AddPrototype(prototype);
                            }
                        }
                    );
                }
            );

            RemovePrototypeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Entity prototype = parameter as Entity;
                    RemovePrototype(prototype);
                }
            );

            AddSceneCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Scene scene = CreateScene();

                    DialogService.ShowDialog(DialogService.Constants.SceneDialog, scene,
                        (result) =>
                        {
                            if (result == true)
                            {
                                AddScene(scene);
                            }
                        }
                    );
                }
            );

            RemoveSceneCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Scene scene = parameter as Scene;
                    RemoveScene(scene);
                }
            );

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

            AddAssetCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    // TODO: File Chooser
                    Asset asset = new Asset("An Asset");
                    AddAsset(asset);
                }
            );

            RemoveAssetCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Asset asset = parameter as Asset;
                    RemoveAsset(asset);
                }
            );

            RemoveItemCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Entity prototype = parameter as Entity;
                    if (null != prototype)
                    {
                        RemovePrototype(prototype);
                    }
                    else
                    {
                        Scene scene = parameter as Scene;
                        if (null != scene)
                        {
                            RemoveScene(scene);
                        }
                    }
                }
            );
        }

        public void AddUsing(Using use)
        {
            use.DefineAdded += OnDefineAdded;
            use.DefineChanged += OnDefineChanged;
            Usings.Add(use);
        }

        public void RemoveUsing(Using use)
        {
            use.DefineAdded -= OnDefineAdded;
            use.DefineChanged -= OnDefineChanged;
            Usings.Remove(use);
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
                prototype.SetScope(this);

                prototype.PluginAdded += OnPluginAdded;
                foreach (Plugin plugin in prototype.Plugins)
                {
                    DefinePlugin(plugin);
                }

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
            prototype.SetScope(null);
            prototype.PluginAdded -= OnPluginAdded;
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
            scene.SetScope(this);

            scene.PluginAdded += OnPluginAdded;
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
            scene.SetScope(null);
            scene.PluginAdded -= OnPluginAdded;
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

        public void AddAttribute(Attribute attribute)
        {
            attribute.SetScope(this);
            Attributes.Add(attribute);

            Workspace.Instance.CommandHistory.Log(
                "add attribute '" + attribute.Key + "'",
                () => AddAttribute(attribute),
                () => RemoveAttribute(attribute)
            );
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

        public void AddAsset(Asset asset)
        {
            Assets.Add(asset);

            Workspace.Instance.CommandHistory.Log(
                "add asset '" + asset.FileName + "'",
                () => AddAsset(asset),
                () => RemoveAsset(asset)
            );
        }

        public void RemoveAsset(Asset asset)
        {
            Assets.Remove(asset);

            Workspace.Instance.CommandHistory.Log(
                "remove asset '" + asset.FileName + "'",
                () => RemoveAsset(asset),
                () => AddAsset(asset)
            );
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

        private void OnPluginAdded(Plugin plugin)
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

        private void OnDefineAdded(Define define)
        {
            if (null != DefineAdded)
            {
                DefineAdded(define);
            }
        }

        private void OnDefineChanged(Define define)
        {
            if (null != DefineChanged)
            {
                Plugin plugin = GetPlugin(define.Class);
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

        string IPluginNamespace.GetDefinedName(Plugin plugin)
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
    }
}
