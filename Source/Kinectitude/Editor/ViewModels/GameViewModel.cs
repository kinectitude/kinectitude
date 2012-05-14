using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Views;
using Kinectitude.Editor.Storage;
using System.Collections.Generic;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.Commands.Game;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.Models.Base;

namespace Kinectitude.Editor.ViewModels
{
    public class GameViewModel : BaseModel
    {
        public const string DefaultName = "Untitled Game";

        private string fileName;
        private readonly Game game;
        private readonly ObservableCollection<AttributeViewModel> _attributes;
        private readonly ObservableCollection<EntityViewModel> _prototypes;
        private readonly ObservableCollection<SceneViewModel> _scenes;
        private readonly ModelCollection<AttributeViewModel> attributes;
        private readonly ModelCollection<EntityViewModel> prototypes;
        private readonly ModelCollection<SceneViewModel> scenes;
        private readonly IPluginNamespace pluginNamespace;

        public Game Game
        {
            get { return game; }
        }

        public string FileName
        {
            get { return fileName; }
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    RaisePropertyChanged("FileName");
                }
            }
        }

        public string Name
        {
            get
            {
                return null != game.Name ? game.Name : DefaultName;
            }
            set
            {
                if (game.Name != value)
                {
                    CommandHistory.Instance.LogCommand(new RenameGameCommand(this, value));
                    game.Name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public string Description
        {
            get { return game.Description; }
            set
            {
                if (game.Description != value)
                {
                    CommandHistory.Instance.LogCommand(new SetDescriptionCommand(this, value));
                    game.Description = value;
                    RaisePropertyChanged("Description");
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
                    CommandHistory.Instance.LogCommand(new SetResolutionCommand(this, value, Height));
                    game.Width = value;
                    RaisePropertyChanged("Width");
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
                    CommandHistory.Instance.LogCommand(new SetResolutionCommand(this, Width, value));
                    game.Height = value;
                    RaisePropertyChanged("Height");
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
                    CommandHistory.Instance.LogCommand(new SetFullScreenCommand(this, value));
                    game.IsFullScreen = value;
                    RaisePropertyChanged("IsFullScreen");
                }
            }
        }

        public SceneViewModel FirstScene
        {
            get { return scenes.FirstOrDefault(x => x.Scene == game.FirstScene); }
            set
            {
                if (null != value)
                {
                    CommandHistory.Instance.LogCommand(new SetFirstSceneCommand(this, value));
                    game.FirstScene = value.Scene;
                    RaisePropertyChanged("FirstScene");
                }
            }
        }

        public ModelCollection<AttributeViewModel> Attributes
        {
            get { return attributes; }
        }

        public ModelCollection<EntityViewModel> Prototypes
        {
            get { return prototypes; }
        }

        public ModelCollection<SceneViewModel> Scenes
        {
            get { return scenes; }
        }

        public ICommand CreateSceneCommand
        {
            get { return new DelegateCommand(null, ExecuteCreateSceneCommand); }
        }

        public ICommand CreatePrototypeCommand
        {
            get { return new DelegateCommand(null, ExecuteCreatePrototypeCommand); }
        }

        public ICommand DeleteItemCommand
        {
            get { return new DelegateCommand(null, ExecuteDeleteItemCommand); }
        }

        public GameViewModel(Game game, IPluginNamespace pluginNamespace)
        {
            this.game = game;
            this.pluginNamespace = pluginNamespace;

            var attributeViewModels = from attribute in game.Attributes select AttributeViewModel.GetViewModel(attribute);
            var prototypeViewModels = from entity in game.Entities select EntityViewModel.GetViewModel(entity);
            var sceneViewModels = from scene in game.Scenes select SceneViewModel.GetViewModel(scene);

            _attributes = new ObservableCollection<AttributeViewModel>(attributeViewModels);
            _prototypes = new ObservableCollection<EntityViewModel>(prototypeViewModels);
            _scenes = new ObservableCollection<SceneViewModel>(sceneViewModels);

            attributes = new ModelCollection<AttributeViewModel>(_attributes);
            prototypes = new ModelCollection<EntityViewModel>(_prototypes);
            scenes = new ModelCollection<SceneViewModel>(_scenes);
        }

        public void SaveGame()
        {
            if (null == FileName)
            {
                SaveGameAs();
            }
            else
            {
                IGameStorage storage = new XmlGameStorage(FileName, pluginNamespace);
                storage.SaveGame(game);
            }
        }

        public void SaveGameAs()
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "game";
            dialog.DefaultExt = ".xml";
            dialog.Filter = "Kinectitude XML Files (.xml)|*.xml";

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                FileName = dialog.FileName;
                IGameStorage storage = new XmlGameStorage(FileName, pluginNamespace);
                storage.SaveGame(game);
            }
        }

        public void ExecuteCreateSceneCommand(object parameter)
        {
            Scene scene = new Scene();
            SceneViewModel viewModel = SceneViewModel.GetViewModel(scene);

            SceneDialog dialog = new SceneDialog();
            dialog.DataContext = viewModel;
            dialog.ShowDialog();

            if (dialog.DialogResult == true)
            {
                AddScene(viewModel);
            }
        }

        public void ExecuteCreatePrototypeCommand(object parameter)
        {
            Entity entity = new Entity();
            EntityViewModel viewModel = EntityViewModel.GetViewModel(entity);

            EntityDialog dialog = new EntityDialog();
            dialog.DataContext = viewModel;
            dialog.ShowDialog();

            if (dialog.DialogResult == true)
            {
                AddPrototype(viewModel);
            }
        }

        public void ExecuteDeleteItemCommand(object parameter)
        {
            SceneViewModel scene = parameter as SceneViewModel;
            if (null != scene)
            {
                RemoveScene(scene);
            }
            else
            {
                EntityViewModel entity = parameter as EntityViewModel;
                if (null != entity)
                {
                    RemovePrototype(entity);
                }
            }
        }

        public void AddPrototype(EntityViewModel prototype)
        {
            CommandHistory.Instance.LogCommand(new CreatePrototypeCommand(this, prototype));
            game.AddEntity(prototype.Entity);
            _prototypes.Add(prototype);
        }

        public void RemovePrototype(EntityViewModel prototype)
        {
            CommandHistory.Instance.LogCommand(new DeletePrototypeCommand(this, prototype));
            _prototypes.Remove(prototype);
            game.RemoveEntity(prototype.Entity);
        }

        public void AddScene(SceneViewModel scene)
        {
            CommandHistory.Instance.LogCommand(new CreateSceneCommand(this, scene));
            game.AddScene(scene.Scene);
            _scenes.Add(scene);
        }

        public void RemoveScene(SceneViewModel scene)
        {
            CommandHistory.Instance.LogCommand(new DeleteSceneCommand(this, scene));
            _scenes.Remove(scene);
            game.RemoveScene(scene.Scene);
        }
    }
}
