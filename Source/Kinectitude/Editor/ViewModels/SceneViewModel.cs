using System.ComponentModel;
using System.Windows.Input;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models;
using Attribute = Kinectitude.Editor.Models.Attribute;
using System.Collections.ObjectModel;

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class SceneViewModel : BaseModel
    {
        private readonly Scene scene;

        public string Name
        {
            get { return scene.Name; }
            set { scene.Name = value; }
        }

        public ObservableCollection<Attribute> Attributes
        {
            get { return scene.Attributes; }
        }

        public ComputedObservableCollection<Entity, EntityViewModel> Entities
        {
            get;
            private set;
        }

        public ICommand SelectCommand { get; private set; }
        public ICommand AddAttributeCommand { get; private set; }
        public ICommand RemoveAttributeCommand { get; private set; }
        public ICommand AddEntityCommand { get; private set; }
        public ICommand RemoveEntityCommand { get; private set; }

        public Scene Model
        {
            get { return scene; }
        }

        public SceneViewModel(Scene scene)
        {
            this.scene = scene;

            Entities = new ComputedObservableCollection<Entity, EntityViewModel>(scene.Entities, (entity) => new EntityViewModel(entity));

            scene.PropertyChanged += Scene_PropertyChanged;

            AddAttributeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    scene.CreateAttribute();
                }
            );

            RemoveAttributeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Attribute attribute = parameter as Attribute;
                    scene.RemoveAttribute(attribute);
                }
            );

            AddEntityCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Entity entity = new Entity();
                    scene.AddEntity(entity);
                }
            );

            RemoveEntityCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Entity entity = parameter as Entity;
                    scene.RemoveEntity(entity);
                }
            );
        }

        private void Scene_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }
    }
}
