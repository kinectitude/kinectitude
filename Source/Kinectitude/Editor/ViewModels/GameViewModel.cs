using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Editor.Base;
using Editor.Views;
using Editor.Storage;
using System.Collections.Generic;
using Editor.Commands.Base;
using Editor.Commands.Game;

namespace Editor.ViewModels
{
    public class GameViewModel : BaseModel
    {
        private static readonly Dictionary<Game, GameViewModel> viewModels;

        static GameViewModel()
        {
            viewModels = new Dictionary<Game, GameViewModel>();
        }

        public static GameViewModel Create(Game game, IPluginFactory pluginFactory)
        {
            if (!viewModels.ContainsKey(game))
            {
                GameViewModel viewModel = new GameViewModel(game, pluginFactory);
                viewModels[game] = viewModel;
            }
            return viewModels[game];
        }

        public static class Constants
        {
            public const string DefaultName = "Untitled Game";
        }

        private string fileName;

        private readonly Game game;
        private readonly ObservableCollection<AttributeViewModel> _attributes;
        private readonly ObservableCollection<EntityViewModel> _prototypes;
        private readonly ObservableCollection<SceneViewModel> _scenes;
        private readonly ModelCollection<AttributeViewModel> attributes;
        private readonly ModelCollection<EntityViewModel> prototypes;
        private readonly ModelCollection<SceneViewModel> scenes;
        private readonly IPluginFactory pluginFactory;
        private readonly ICommandHistory commandHistory;
        
        public event EventHandler UndoCountChanged = delegate { };
        public event EventHandler RedoCountChanged = delegate { };

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
                return null != game.Name ? game.Name : Constants.DefaultName;
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

        public ICommandHistory CommandHistory
        {
            get { return commandHistory; }
        }

        public ICommand CreateSceneCommand
        {
            get { return new DelegateCommand(null, ExecuteCreateSceneCommand); }
        }

        public ICommand CreatePrototypeCommand
        {
            get { return new DelegateCommand(null, ExecuteCreatePrototypeCommand); }
        }

        public ICommand RenameSceneCommand
        {
            get { return new DelegateCommand(null, ExecuteRenameSceneCommand); }
        }

        public ICommand DeleteItemCommand
        {
            get { return new DelegateCommand(null, ExecuteDeleteItemCommand); }
        }

        private GameViewModel(Game game, IPluginFactory pluginFactory)
        {
            this.game = game;
            this.pluginFactory = pluginFactory;

            var attributeViewModels = from attribute in game.Attributes select new AttributeViewModel(attribute);
            _attributes = new ObservableCollection<AttributeViewModel>(attributeViewModels);
            attributes = new ModelCollection<AttributeViewModel>(_attributes);

            var prototypeViewModels = from entity in game.Prototypes select EntityViewModel.Create(entity);
            _prototypes = new ObservableCollection<EntityViewModel>(prototypeViewModels);
            prototypes = new ModelCollection<EntityViewModel>(_prototypes);

            var sceneViewModels = from scene in game.Scenes select SceneViewModel.Create(game, scene);
            _scenes = new ObservableCollection<SceneViewModel>(sceneViewModels);
            scenes = new ModelCollection<SceneViewModel>(_scenes);

            commandHistory = new CommandHistory();
        }

        public void SaveGame()
        {
            if (null == FileName)
            {
                SaveGameAs();
            }
            else
            {
                IGameStorage storage = new XmlGameStorage(FileName, pluginFactory);
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
                IGameStorage storage = new XmlGameStorage(FileName, pluginFactory);
                storage.SaveGame(game);
            }
        }

        public void ExecuteCreateSceneCommand(object parameter)
        {
            Scene scene = new Scene();
            SceneViewModel viewModel = SceneViewModel.Create(game, scene);

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
            EntityViewModel viewModel = EntityViewModel.Create(entity);

            EntityDialog dialog = new EntityDialog();
            dialog.DataContext = viewModel;
            dialog.ShowDialog();

            if (dialog.DialogResult == true)
            {
                CreatePrototypeCommand command = new CreatePrototypeCommand(commandHistory, this, viewModel);
                command.Execute();
            }
        }

        public void ExecuteRenameSceneCommand(object parameter)
        {
            // This should really be in the property setter for SceneViewModel::Name

            /*SceneViewModel viewModel = parameter as SceneViewModel;
            if (null != viewModel)
            {
                string oldName = viewModel.Name;
                SceneDialog dialog = new SceneDialog();
                dialog.DataContext = viewModel;
                dialog.ShowDialog();

                if (dialog.DialogResult == true)
                {
                    RenameSceneCommand command = new RenameSceneCommand(this, viewModel, oldName);
                    command.Execute();
                }
            }*/
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
