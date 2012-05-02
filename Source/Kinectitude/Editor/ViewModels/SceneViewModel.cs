using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Base;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Kinectitude.Editor.Commands;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.Commands.Scene;

namespace Kinectitude.Editor.ViewModels
{
    public class SceneViewModel : BaseModel
    {
        private static readonly Dictionary<Scene, SceneViewModel> viewModels;

        static SceneViewModel()
        {
            viewModels = new Dictionary<Scene, SceneViewModel>();
        }

        public static SceneViewModel Create(Game game, Scene scene)
        {
            if (!viewModels.ContainsKey(scene))
            {
                SceneViewModel viewModel = new SceneViewModel(game, scene);
                viewModels[scene] = viewModel;
            }
            return viewModels[scene];
        }
        
        private EntityViewModel currentEntity;

        private readonly Game game;
        private readonly Scene scene;
        private readonly ObservableCollection<AttributeViewModel> _attributes;
        private readonly ObservableCollection<EntityViewModel> _entities;
        private readonly ObservableCollection<EventViewModel> _events;
        private readonly ModelCollection<AttributeViewModel> attributes;
        private readonly ModelCollection<EntityViewModel> entities;
        private readonly ModelCollection<EventViewModel> events;
        private readonly ICommandHistory commandHistory;

        public Scene Scene
        {
            get { return scene; }
        }

        public string Name
        {
            get { return scene.Name; }
            set
            {
                RenameSceneCommand command = new RenameSceneCommand(commandHistory, this, value);
                command.Execute();
            }
        }

        public EntityViewModel CurrentEntity
        {
            get { return currentEntity; }
            set
            {
                SelectEntityCommand command = new SelectEntityCommand(commandHistory, this, value);
                command.Execute();
            }
        }

        public ModelCollection<AttributeViewModel> Attributes
        {
            get { return attributes; }
        }

        public ModelCollection<EntityViewModel> Entities
        {
            get { return entities; }
        }

        public ModelCollection<EventViewModel> Events
        {
            get { return events; }
        }

        public int Width
        {
            get { return game.Width; }
        }

        public int Height
        {
            get { return game.Height; }
        }

        public ICommandHistory CommandHistory
        {
            get { return commandHistory; }
        }

        private SceneViewModel(Game game, Scene scene)
        {
            this.game = game;
            this.scene = scene;

            var attributeViewModels = from attribute in scene.Attributes select new AttributeViewModel(attribute);
            var entityViewModels = from entity in scene.Entities select EntityViewModel.Create(entity);
            //var eventViewModels = from evt in scene.Events select new EventViewModel(evt);

            _attributes = new ObservableCollection<AttributeViewModel>(attributeViewModels);
            _entities = new ObservableCollection<EntityViewModel>(entityViewModels);
            //_events = new ObservableCollection<EventViewModel>(eventViewModels);

            attributes = new ModelCollection<AttributeViewModel>(_attributes);
            entities = new ModelCollection<EntityViewModel>(_entities);
            //events = new ModelCollection<EventViewModel>(_events);

            commandHistory = new CommandHistory();
        }
    }
}
