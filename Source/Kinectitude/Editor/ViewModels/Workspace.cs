﻿using System;
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

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class Workspace : BaseViewModel
    {
        private const string PluginDirectory = "Plugins";

        private static readonly Lazy<Workspace> instance = new Lazy<Workspace>();

        private readonly Lazy<CommandHistory> commandHistory;

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
            OpenItems = new ObservableCollection<BaseViewModel>();
            Plugins = new ObservableCollection<PluginViewModel>();

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

            OpenItemCommand = new DelegateCommand(null, (parameter) => OpenItem(parameter as BaseViewModel));

            CloseItemCommand = new DelegateCommand(null, (parameter) => CloseItem(parameter as BaseViewModel));

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
        }

        public void NewGame()
        {
            Game = new GameViewModel("Untitled Game") { Width = 800, Height = 600 };
            Game.AddScene(new SceneViewModel("Scene 1"));
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

        public void OpenItem(BaseViewModel item)
        {
            if (!OpenItems.Contains(item))
            {
                OpenItems.Add(item);
            }

            ActiveItem = item;
        }

        public void CloseItem(BaseViewModel item)
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
