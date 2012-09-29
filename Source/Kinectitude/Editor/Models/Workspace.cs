using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Kinectitude.Core.Attributes;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Commands;
using Kinectitude.Editor.Storage;
using Kinectitude.Editor.Views;
using System.Windows.Input;
using System.Collections.Specialized;
using Kinectitude.Editor.Presenters;
using System.Collections.Generic;
using Kinectitude.Core.Components;
using Kinectitude.Render;

namespace Kinectitude.Editor.Models
{
    internal sealed class Workspace : BaseModel
    {
        private const string PluginDirectory = "Plugins";

        private static readonly Lazy<Workspace> instance = new Lazy<Workspace>();

        private readonly Lazy<CommandHistory> commandHistory;

        public static Workspace Instance
        {
            get { return instance.Value; }
        }

        private Game game;
        private BaseModel activeItem;
        private List<EntityPreset> entityPresets;

        public Game Game
        {
            get { return game; }
            set
            {
                if (game != value)
                {
                    game = value;
                    NotifyPropertyChanged("Game");
                }
            }
        }

        public BaseModel ActiveItem
        {
            get { return activeItem; }
            set
            {
                if (activeItem != value)
                {
                    activeItem = value;
                    NotifyPropertyChanged("ActiveItem");
                }
            }
        }

        public ObservableCollection<BaseModel> OpenItems
        {
            get;
            private set;
        }

        public ObservableCollection<Plugin> Plugins
        {
            get;
            private set;
        }

        public ObservableCollection<Plugin> Actions
        {
            get;
            private set;
        }

        public ObservableCollection<Plugin> Events
        {
            get;
            private set;
        }

        public ObservableCollection<Plugin> Components
        {
            get;
            private set;
        }

        public IEnumerable<EntityPreset> EntityPresets
        {
            get { return entityPresets; }
        }

        public ICommand NewGameCommand
        {
            get;
            private set;
        }

        public ICommand LoadGameCommand
        {
            get;
            private set;
        }

        public ICommand SaveGameCommand
        {
            get;
            private set;
        }

        public ICommand SaveGameAsCommand
        {
            get;
            private set;
        }

        public ICommand OpenItemCommand
        {
            get;
            private set;
        }

        public ICommand CloseItemCommand
        {
            get;
            private set;
        }

        public ICommandHistory CommandHistory
        {
            get { return commandHistory.Value; }
        }

        public Workspace()
        {
            OpenItems = new ObservableCollection<BaseModel>();
            Plugins = new ObservableCollection<Plugin>();

            Actions = new FilteredObservableCollection<Plugin>(Plugins, (plugin) => plugin.Type == PluginType.Action);
            Events = new FilteredObservableCollection<Plugin>(Plugins, (plugin) => plugin.Type == PluginType.Event);
            Components = new FilteredObservableCollection<Plugin>(Plugins, (plugin) => plugin.Type == PluginType.Component);

            commandHistory = new Lazy<CommandHistory>();

            NewGameCommand = new DelegateCommand(null, (parameter) => NewGame());

            LoadGameCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    DialogService.ShowLoadDialog(
                        (result, fileName) =>
                        {
                            if (result == true)
                            {
                                LoadGame(fileName);
                            }
                        }
                    );
                }
            );

            SaveGameCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    if (null == Game.FileName)
                    {
                        DialogService.ShowSaveDialog(
                            (result, fileName) =>
                            {
                                if (result == true)
                                {
                                    Game.FileName = fileName;
                                }
                            }
                        );
                    }

                    if (null != Game.FileName)
                    {
                        SaveGame();
                    }
                }
            );

            SaveGameAsCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    DialogService.ShowSaveDialog(
                        (result, fileName) =>
                        {
                            if (result == true)
                            {
                                Game.FileName = fileName;
                                SaveGame();
                            }
                        }
                    );
                }
            );

            OpenItemCommand = new DelegateCommand(null, (parameter) => OpenItem(parameter as BaseModel));

            CloseItemCommand = new DelegateCommand(null, (parameter) => CloseItem(parameter as BaseModel));

            Assembly core = typeof(Kinectitude.Core.Base.Component).Assembly;
            RegisterPlugins(core);

            DirectoryInfo path = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, PluginDirectory));
            if (path.Exists)
            {
                FileInfo[] files = path.GetFiles("*.dll");
                foreach (FileInfo file in files)
                {
                    Assembly asm = Assembly.LoadFrom(file.FullName);
                    RegisterPlugins(asm);
                }
            }

            entityPresets = new List<EntityPreset>();
            entityPresets.Add(new EntityPreset("Blank Entity", GetPlugin(typeof(TransformComponent))));
            entityPresets.Add(new EntityPreset("Image Entity", GetPlugin(typeof(TransformComponent)), GetPlugin(typeof(ImageRenderComponent))));
            entityPresets.Add(new EntityPreset("Shape Entity", GetPlugin(typeof(TransformComponent)), GetPlugin(typeof(RenderComponent))));
            entityPresets.Add(new EntityPreset("Text Entity", GetPlugin(typeof(TransformComponent)), GetPlugin(typeof(TextRenderComponent))));
        }

        public void NewGame()
        {
            Game game = new Game("Untitled Game") { Width = 800, Height = 600 };
            game.AddScene(new Scene("Scene 1"));

            Game = game;
        }

        public void LoadGame(string fileName)
        {
            IGameStorage storage = new XmlGameStorage(fileName);
            Game = storage.LoadGame();
        }

        public void SaveGame()
        {
            IGameStorage storage = new XmlGameStorage(Game.FileName);
            storage.SaveGame(Game);
        }

        public void OpenItem(BaseModel item)
        {
            if (!OpenItems.Contains(item))
            {
                OpenItems.Add(item);
            }

            ActiveItem = item;
        }

        public void CloseItem(BaseModel item)
        {
            OpenItems.Remove(item);

            if (ActiveItem == item)
            {
                ActiveItem = OpenItems.FirstOrDefault();
            }
        }

        private void RegisterPlugins(Assembly assembly)
        {
            var types = from type in assembly.GetTypes()
                        where System.Attribute.IsDefined(type, typeof(PluginAttribute)) &&
                        (
                            typeof(Kinectitude.Core.Base.Component) != type && typeof(Kinectitude.Core.Base.Component).IsAssignableFrom(type) ||
                            typeof(Kinectitude.Core.Base.Event) != type && typeof(Kinectitude.Core.Base.Event).IsAssignableFrom(type) ||
                            typeof(Kinectitude.Core.Base.Action) != type && typeof(Kinectitude.Core.Base.Action).IsAssignableFrom(type) ||
                            typeof(Kinectitude.Core.Base.IManager) != type && typeof(Kinectitude.Core.Base.IManager).IsAssignableFrom(type)
                        )
                        select new Plugin(type);

            foreach (Plugin plugin in types)
            {
                AddPlugin(plugin);
            }
        }

        public void AddPlugin(Plugin plugin)
        {
            Plugins.Add(plugin);
        }

        public void RemovePlugin(Plugin plugin)
        {
            Plugins.Remove(plugin);
        }

        public Plugin GetPlugin(string name)
        {
            Plugin plugin = Plugins.FirstOrDefault(x => x.ClassName == name);
            
            if (null == plugin)
            {
                plugin = Plugins.FirstOrDefault(x => x.File == typeof(Kinectitude.Core.Base.Component).Module.Name && x.ShortName == name);
            }
            
            return plugin;
        }

        public Plugin GetPlugin(Type type)
        {
            return GetPlugin(type.FullName);
        }
    }
}
