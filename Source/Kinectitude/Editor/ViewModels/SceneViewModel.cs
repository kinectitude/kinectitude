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
using Kinectitude.Editor.Models.Base;

namespace Kinectitude.Editor.ViewModels
{
    public class SceneViewModel : BaseModel
    {
        private static readonly Dictionary<Scene, SceneViewModel> sceneViewModels;

        static SceneViewModel()
        {
            sceneViewModels = new Dictionary<Scene, SceneViewModel>();
        }

        public static SceneViewModel GetViewModel(Scene scene)
        {
            SceneViewModel sceneViewModel = null;
            sceneViewModels.TryGetValue(scene, out sceneViewModel);
            if (null == sceneViewModel)
            {
                sceneViewModel = new SceneViewModel(scene);
                sceneViewModels[scene] = sceneViewModel;
            }
            return sceneViewModel;
        }

        private EntityViewModel currentEntity;

        private readonly Scene scene;
        private readonly ObservableCollection<AttributeViewModel> _attributes;
        private readonly ObservableCollection<EntityViewModel> _entities;
        private readonly ModelCollection<AttributeViewModel> attributes;
        private readonly ModelCollection<EntityViewModel> entities;
        
        public Scene Scene
        {
            get { return scene; }
        }

        public string Name
        {
            get { return scene.Name; }
            set
            {
                CommandHistory.Instance.LogCommand(new RenameSceneCommand(this, value));
                scene.Name = value;
                RaisePropertyChanged("Name");
            }
        }

        public EntityViewModel CurrentEntity
        {
            get { return currentEntity; }
            set
            {
                //SelectEntityCommand command = new SelectEntityCommand(this, value);
                //command.Execute();
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

        public SceneViewModel(Scene scene)
        {
            this.scene = scene;

            var attributeViewModels = from attribute in scene.Attributes select AttributeViewModel.GetViewModel(attribute);
            var entityViewModels = from entity in scene.Entities select EntityViewModel.GetViewModel(entity);

            _attributes = new ObservableCollection<AttributeViewModel>(attributeViewModels);
            _entities = new ObservableCollection<EntityViewModel>(entityViewModels);

            attributes = new ModelCollection<AttributeViewModel>(_attributes);
            entities = new ModelCollection<EntityViewModel>(_entities);
        }
    }
}
