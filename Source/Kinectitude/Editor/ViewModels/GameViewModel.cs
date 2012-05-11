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
        private readonly ICommandHistory commandHistory;

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
                    RenameGameCommand command = new RenameGameCommand(commandHistory, this, value);
                    command.Execute();
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
                    SetDescriptionCommand command = new SetDescriptionCommand(commandHistory, this, value);
                    command.Execute();
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
                    SetResolutionCommand command = new SetResolutionCommand(commandHistory, this, value, Height);
                    command.Execute();
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
                    SetResolutionCommand command = new SetResolutionCommand(commandHistory, this, Width, value);
                    command.Execute();
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
                    SetFullScreenCommand command = new SetFullScreenCommand(commandHistory, this, value);
                    command.Execute();
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
                    SetFirstSceneCommand command = new SetFirstSceneCommand(commandHistory, this, value);
                    command.Execute();
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

        public GameViewModel(Game game, IPluginNamespace pluginNamespace, ICommandHistory commandHistory)
        {
            this.game = game;
            this.pluginNamespace = pluginNamespace;
            this.commandHistory = commandHistory;

            var attributeViewModels = from attribute in game.Attributes select AttributeViewModel.GetViewModel(attribute);
            var prototypeViewModels = from entity in game.Entities select EntityViewModel.GetViewModel(entity);  // TODO: Make these available to Scenes
            var sceneViewModels = from scene in game.Scenes select new SceneViewModel(scene, commandHistory);

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
            SceneViewModel viewModel = new SceneViewModel(scene, commandHistory);

            SceneDialog dialog = new SceneDialog();
            dialog.DataContext = viewModel;
            dialog.ShowDialog();

            if (dialog.DialogResult == true)
            {
                CreateSceneCommand command = new CreateSceneCommand(commandHistory, this, viewModel);
                command.Execute();
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
                CreatePrototypeCommand command = new CreatePrototypeCommand(commandHistory, this, viewModel);
                command.Execute();
            }
        }

        public void ExecuteDeleteItemCommand(object parameter)
        {
            SceneViewModel scene = parameter as SceneViewModel;
            if (null != scene)
            {
                DeleteSceneCommand command = new DeleteSceneCommand(commandHistory, this, scene);
                command.Execute();
            }
            else
            {
                EntityViewModel entity = parameter as EntityViewModel;
                if (null != entity)
                {
                    DeletePrototypeCommand command = new DeletePrototypeCommand(commandHistory, this, entity);
                    command.Execute();
                }
            }
        }

        public void AddPrototype(EntityViewModel prototype)
        {
            game.AddEntity(prototype.Entity);
            _prototypes.Add(prototype);
        }

        public void RemovePrototype(EntityViewModel prototype)
        {
            _prototypes.Remove(prototype);
            game.RemoveEntity(prototype.Entity);
        }

        public EntityViewModel GetPrototype(Entity prototype)
        {
            return prototypes.FirstOrDefault(x => x.Entity == prototype);
        }

        public void AddScene(SceneViewModel scene)
        {
            game.AddScene(scene.Scene);
            _scenes.Add(scene);
        }

        public void RemoveScene(SceneViewModel scene)
        {
            _scenes.Remove(scene);
            game.RemoveScene(scene.Scene);
        }
    }
}
