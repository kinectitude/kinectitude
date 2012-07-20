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

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class Workspace : BaseViewModel
    {
        private const string PluginDirectory = "Plugins";

        private static Lazy<Workspace> instance = new Lazy<Workspace>();

        public static Workspace Instance
        {
            get { return instance.Value; }
        }

        private GameViewModel game;
        private BaseViewModel activeItem;

        public GameViewModel Game
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

        public BaseViewModel ActiveItem
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

        public ObservableCollection<BaseViewModel> OpenItems
        {
            get;
            private set;
        }

        public ObservableCollection<PluginViewModel> Plugins
        {
            get;
            private set;
        }

        public ICommandHistory CommandHistory
        {
            get;
            private set;
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

        public Workspace()
        {
            OpenItems = new ObservableCollection<BaseViewModel>();
            Plugins = new ObservableCollection<PluginViewModel>();
            CommandHistory = new CommandHistory();

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

            Assembly core = typeof(Kinectitude.Core.Base.Component).Assembly;
            RegisterPluginsFromAssembly(core);

            DirectoryInfo path = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, PluginDirectory));
            if (path.Exists)
            {
                FileInfo[] files = path.GetFiles("*.dll");
                foreach (FileInfo file in files)
                {
                    Assembly asm = Assembly.LoadFrom(file.FullName);
                    RegisterPluginsFromAssembly(asm);
                }
            }
        }

        public void NewGame()
        {
            Game = new GameViewModel("Untitled Game");
            Game.AddScene(new SceneViewModel("Scene 1"));
        }

        public void LoadGame(string fileName)
        {
            IGameStorage storage = new XmlGameStorage(fileName);
            Game = storage.LoadGame();
        }

        private void RegisterPluginsFromAssembly(Assembly assembly)
        {
            var types = from type in assembly.GetTypes()
                        where System.Attribute.IsDefined(type, typeof(PluginAttribute)) &&
                        (
                            typeof(Kinectitude.Core.Base.Component) != type && typeof(Kinectitude.Core.Base.Component).IsAssignableFrom(type) ||
                            typeof(Kinectitude.Core.Base.Event) != type && typeof(Kinectitude.Core.Base.Event).IsAssignableFrom(type) ||
                            typeof(Kinectitude.Core.Base.Action) != type && typeof(Kinectitude.Core.Base.Action).IsAssignableFrom(type) ||
                            typeof(Kinectitude.Core.Base.IManager) != type && typeof(Kinectitude.Core.Base.IManager).IsAssignableFrom(type)
                        )
                        select new PluginViewModel(type);

            foreach (PluginViewModel plugin in types)
            {
                AddPlugin(plugin);
            }
        }

        public void AddPlugin(PluginViewModel plugin)
        {
            Plugins.Add(plugin);
        }

        public void RemovePlugin(PluginViewModel plugin)
        {
            Plugins.Remove(plugin);
        }

        public PluginViewModel GetPlugin(string name)
        {
            PluginViewModel plugin = Plugins.FirstOrDefault(x => x.ClassName == name);
            
            if (null == plugin)
            {
                plugin = Plugins.FirstOrDefault(x => x.File == typeof(Kinectitude.Core.Base.Component).Module.Name && x.ShortName == name);
            }
            
            return plugin;
        }
    }
}
