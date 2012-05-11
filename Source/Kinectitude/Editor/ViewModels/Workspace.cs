using System;
using System.Collections.Generic;
using System.Linq;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Storage;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Windows;
using Kinectitude.Attributes;
using Kinectitude.Editor.Models.Plugins;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.Models.Base;
using Kinectitude.Editor.Commands.Base;

namespace Kinectitude.Editor.ViewModels
{
    public sealed class Workspace : BaseModel, IPluginNamespace
    {
        public const string PluginDirectory = "Plugins";

        private readonly List<PluginViewModel> plugins;
        private readonly ObservableCollection<BaseModel> _activeItems;
        private readonly ModelCollection<BaseModel> activeItems;
        private readonly ResolutionPreset[] resolutionPresets;
        private readonly ICommandHistory commandHistory;

        private GameViewModel game;
        private BaseModel currentItem;

        public ICommand NewCommand
        {
            get { return new DelegateCommand(null, NewGame); }
        }

        public ICommand LoadCommand
        {
            get { return new DelegateCommand(null, LoadGame); }
        }

        public ICommand SaveCommand
        {
            get { return new DelegateCommand(null, SaveGame); }
        }

        public ICommand SaveAsCommand
        {
            get { return new DelegateCommand(null, SaveGameAs); }
        }

        public ICommand RevertCommand
        {
            get { return new DelegateCommand(null, RevertGame); }
        }

        public ICommand OpenItemCommand
        {
            get { return new DelegateCommand(null, OpenItem); }
        }

        public ICommand CloseItemCommand
        {
            get { return new DelegateCommand(null, CloseItem); }
        }

        public ICommand ExitCommand
        {
            get { return new DelegateCommand(null, Exit); }
        }

        public GameViewModel Game
        {
            get { return game; }
            set
            {
                if (game != value)
                {
                    game = value;
                    RaisePropertyChanged("Game");
                }
            }
        }

        public IEnumerable<PluginViewModel> Plugins
        {
            get { return plugins; }
        }

        public BaseModel CurrentItem
        {
            get { return currentItem; }
            set
            {
                if (currentItem != value)
                {
                    currentItem = value;
                    RaisePropertyChanged("CurrentItem");
                }
            }
        }

        public ModelCollection<BaseModel> ActiveItems
        {
            get { return activeItems; }
        }

        public IEnumerable<ResolutionPreset> ResolutionPresets
        {
            get { return resolutionPresets; }
        }

        public ICommandHistory CommandHistory
        {
            get { return commandHistory; }
        }

        public Workspace()
        {
            plugins = new List<PluginViewModel>();
            commandHistory = new CommandHistory();

            Assembly core = typeof(Kinectitude.Core.Base.Component).Assembly;
            registerTypesFromAssembly(core, true);

            string[] files = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, PluginDirectory), "*.dll");
            foreach (string file in files)
            {
                Assembly asm = Assembly.LoadFrom(file);
                registerTypesFromAssembly(asm);
            }
            NewGame(null);

            _activeItems = new ObservableCollection<BaseModel>();
            activeItems = new ModelCollection<BaseModel>(_activeItems);

            resolutionPresets = new ResolutionPreset[]
            {
                new ResolutionPreset("Standard Definition", 640, 480),
                new ResolutionPreset("1080p", 1920, 1080),
                new ResolutionPreset("Custom", 0, 0)
            };
        }

        private void registerTypesFromAssembly(Assembly assembly, bool includeInternalTypes = false)
        {
            IEnumerable<Type> types = from type in assembly.GetTypes()
                                      where
                                      (
                                          (
                                              typeof(Kinectitude.Core.Base.Component) != type &&
                                              typeof(Kinectitude.Core.Base.Component).IsAssignableFrom(type)
                                          ) ||
                                          (
                                              typeof(Kinectitude.Core.Base.Event) != type &&
                                              typeof(Kinectitude.Core.Base.Event).IsAssignableFrom(type)
                                          ) ||
                                          (
                                              typeof(Kinectitude.Core.Base.Action) != type &&
                                              typeof(Kinectitude.Core.Base.Action).IsAssignableFrom(type)
                                          )
                                      ) &&
                                      System.Attribute.IsDefined(type, typeof(PluginAttribute))
                                      select type;

            foreach (Type type in types)
            {
                PluginDescriptor descriptor = new PluginDescriptor(type);
                plugins.Add(new PluginViewModel(descriptor));
            }
        }

        public PluginDescriptor GetPluginDescriptor(string name)
        {
            var descriptors = from viewModel in plugins select viewModel.Descriptor;

            PluginDescriptor ret = null;
            foreach (PluginDescriptor descriptor in descriptors)
            {
                if (descriptor.Name == name)
                {
                    ret = descriptor;
                    break;
                }

                if (descriptor.File == "Kinectitude.Core.dll")
                {
                    string shortName = descriptor.Name.Substring(descriptor.Name.LastIndexOf('.') + 1);
                    if (shortName == name)
                    {
                        ret = descriptor;
                        break;
                    }
                }
            }
            
            if (null == ret)
            {
                throw new PluginNotLoadedException(name);
            }

            return ret;
        }

        public void NewGame(object parameter)
        {
            Game game = new Game(this);
            GameViewModel viewModel = new GameViewModel(game, this, commandHistory);
            Game = viewModel;
        }

        public void LoadGame(object parameter)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = string.Empty;
            dialog.DefaultExt = ".xml";
            dialog.Filter = "Kinectitude XML Files (.xml)|*.xml";

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                loadGameFromFile(dialog.FileName);
            }
        }

        public void SaveGame(object parameter)
        {

        }

        public void SaveGameAs(object parameter)
        {

        }

        public void RevertGame(object parameter)
        {
            loadGameFromFile(Game.FileName);
        }

        public void OpenItem(object parameter)
        {
            BaseModel baseModel = parameter as BaseModel;
            if (null != baseModel && !_activeItems.Contains(baseModel))
            {
                _activeItems.Add(baseModel);
            }
            CurrentItem = baseModel;
        }

        public void CloseItem(object parameter)
        {
            BaseModel baseModel = parameter as BaseModel;
            if (null != baseModel)
            {
                _activeItems.Remove(baseModel);
            }
        }

        public void Exit(object parameter)
        {
            Application.Current.Shutdown();
        }

        private void loadGameFromFile(string fileName)
        {
            IGameStorage storage = new XmlGameStorage(fileName, this);

            try
            {
                Game game = storage.LoadGame();

                GameViewModel gameViewModel = new GameViewModel(game, this, commandHistory);
                gameViewModel.FileName = fileName;
                Game = gameViewModel;
            }
            catch (PluginNotLoadedException e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }
    }
}
