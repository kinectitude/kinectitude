using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Base;
using Editor.Storage;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using Editor.Views;
using System.Windows;
using Kinectitude.Attributes;

namespace Editor.ViewModels
{
    public class Workspace : IPluginFactory, INotifyPropertyChanged
    {
        public const string PluginDirectory = "Plugins";

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private readonly List<PluginViewModel> plugins;
        private readonly ObservableCollection<BaseModel> _activeItems;
        private readonly ModelCollection<BaseModel> activeItems;
        private readonly ResolutionPreset[] resolutionPresets;

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

        public Workspace()
        {
            plugins = new List<PluginViewModel>();

            Assembly core = typeof(Kinectitude.Core.Component).Assembly;
            registerTypesFromAssembly(core);

            string[] files = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, PluginDirectory), "*.dll");
            foreach (string file in files)
            {
                try
                {
                    Assembly asm = Assembly.LoadFrom(file);
                    registerTypesFromAssembly(asm);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
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

        private void registerTypesFromAssembly(Assembly assembly)
        {
            IEnumerable<Type> types = from type in assembly.GetTypes()
                                      where
                                      (
                                          (
                                              typeof(Kinectitude.Core.Component) != type &&
                                              typeof(Kinectitude.Core.Component).IsAssignableFrom(type)
                                          ) ||
                                          (
                                              typeof(Kinectitude.Core.Event) != type &&
                                              typeof(Kinectitude.Core.Event).IsAssignableFrom(type)
                                          ) ||
                                          (
                                              typeof(Kinectitude.Core.Action) != type &&
                                              typeof(Kinectitude.Core.Action).IsAssignableFrom(type)
                                          )
                                      ) &&
                                      Attribute.IsDefined(type, typeof(PluginAttribute))
                                      select type;

            foreach (Type type in types)
            {
                PluginDescriptor descriptor = new PluginDescriptor(type);
                plugins.Add(new PluginViewModel(descriptor));
            }
        }

        private PluginDescriptor getDescriptor(string type)
        {
            PluginViewModel viewModel = plugins.FirstOrDefault(x => x.Descriptor.Class == type);
            if (null == viewModel)
            {
                throw new PluginNotLoadedException(type);
            }
            return viewModel.Descriptor;
        }

        public Component CreateComponent(string name)
        {
            PluginDescriptor descriptor = getDescriptor(name);
            return new Component(descriptor);
        }

        public Event CreateEvent(string name)
        {
            PluginDescriptor descriptor = getDescriptor(name);
            return new Event(descriptor);
        }

        public Action CreateAction(string name)
        {
            PluginDescriptor descriptor = getDescriptor(name);
            return new Action(descriptor);
        }

        protected void NewGame(object parameter)
        {
            Game game = new Game();
            GameViewModel viewModel = GameViewModel.Create(game, this);
            Game = viewModel;
        }

        protected void LoadGame(object parameter)
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

        protected void SaveGame(object parameter)
        {

        }

        protected void SaveGameAs(object parameter)
        {

        }

        protected void RevertGame(object parameter)
        {
            loadGameFromFile(Game.FileName);
        }

        protected void OpenItem(object parameter)
        {
            BaseModel baseModel = parameter as BaseModel;
            if (null != baseModel && !_activeItems.Contains(baseModel))
            {
                _activeItems.Add(baseModel);
            }
        }

        protected void CloseItem(object parameter)
        {
            BaseModel baseModel = parameter as BaseModel;
            if (null != baseModel)
            {
                _activeItems.Remove(baseModel);
            }
        }

        protected void Exit(object parameter)
        {
            Application.Current.Shutdown();
        }

        private void loadGameFromFile(string fileName)
        {
            IGameStorage storage = new XmlGameStorage(fileName, this);

            try
            {
                Game game = storage.LoadGame();

                GameViewModel gameViewModel = GameViewModel.Create(game, this);
                gameViewModel.FileName = fileName;
                Game = gameViewModel;
            }
            catch (PluginNotLoadedException e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
