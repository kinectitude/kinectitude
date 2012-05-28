using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.Commands.Scene;
using Kinectitude.Editor.Models.Base;
using System.Windows.Input;

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

        private string stagedAttributeKey;
        private string stagedAttributeValue;
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
                CommandHistory.LogCommand(new RenameSceneCommand(this, value));
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

        public string StagedAttributeKey
        {
            get { return stagedAttributeKey; }
            set
            {
                if (stagedAttributeKey != value)
                {
                    stagedAttributeKey = value;
                    RaisePropertyChanged("StagedAttributeKey");
                }
            }
        }

        public string StagedAttributeValue
        {
            get { return stagedAttributeValue; }
            set
            {
                if (stagedAttributeValue != value)
                {
                    stagedAttributeValue = value;
                    RaisePropertyChanged("StagedAttributeValue");
                }
            }
        }

        public ICommand AddAttributeCommand
        {
            get { return new DelegateCommand(null, ExecuteAddAttributeCommand); }
        }

        public ICommand RemoveAttributeCommand
        {
            get { return new DelegateCommand(null, ExecuteRemoveAttributeCommand); }
        }

        public SceneViewModel(Scene scene)
        {
            this.scene = scene;

            var attributeViewModels = from attribute in scene.Attributes select AttributeViewModel.GetViewModel(scene, attribute.Key);
            var entityViewModels = from entity in scene.Entities select EntityViewModel.GetViewModel(entity);

            _attributes = new ObservableCollection<AttributeViewModel>(attributeViewModels);
            _entities = new ObservableCollection<EntityViewModel>(entityViewModels);

            attributes = new ModelCollection<AttributeViewModel>(_attributes);
            entities = new ModelCollection<EntityViewModel>(_entities);
        }

        public void ExecuteAddAttributeCommand(object parameter)
        {
            AttributeViewModel attribute = AttributeViewModel.GetViewModel(scene, stagedAttributeKey);
            attribute.Value = stagedAttributeValue;

            AddAttribute(attribute);

            StagedAttributeKey = null;
            StagedAttributeValue = null;
        }

        public void ExecuteRemoveAttributeCommand(object parameter)
        {
            AttributeViewModel attribute = parameter as AttributeViewModel;
            if (null != attribute)
            {
                RemoveAttribute(attribute);
            }
        }

        public void AddAttribute(AttributeViewModel attribute)
        {
            CommandHistory.LogCommand(new AddAttributeCommand(this, attribute));
            attribute.AddAttribute();
            _attributes.Add(attribute);
        }

        public void RemoveAttribute(AttributeViewModel attribute)
        {
            CommandHistory.LogCommand(new RemoveAttributeCommand(this, attribute));
            attribute.RemoveAttribute();
            _attributes.Remove(attribute);
        }
    }
}
