using System.ComponentModel;
using System.Windows.Input;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.Views;
using Attribute = Kinectitude.Editor.Models.Attribute;
using System.Collections.ObjectModel;

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class GameViewModel : BaseModel
    {
        private readonly Game game;
        private SceneViewModel firstScene;

        public string FileName
        {
            get { return game.FileName; }
            set { game.FileName = value; }
        }

        public string Name
        {
            get { return game.Name; }
            set { game.Name = value; }
        }

        public int Width
        {
            get { return game.Width; }
            set { game.Width = value; }
        }

        public int Height
        {
            get { return game.Height; }
            set { game.Height = value; }
        }

        public bool IsFullScreen
        {
            get { return game.IsFullScreen; }
            set { game.IsFullScreen = value; }
        }

        public SceneViewModel FirstScene
        {
            get { return firstScene; }
            set
            {
                firstScene = value;
                game.FirstScene = firstScene.Model;
            }
        }

        public ComputedObservableCollection<Entity, EntityViewModel> Prototypes
        {
            get;
            private set;
        }

        public ComputedObservableCollection<Scene, SceneViewModel> Scenes
        {
            get;
            private set;
        }

        public ObservableCollection<Attribute> Attribute
        {
            get { return game.Attributes; }
        }

        public ICommand AddPrototypeCommand { get; private set; }
        public ICommand RemovePrototypeCommand { get; private set; }
        public ICommand AddAttributeCommand { get; private set; }
        public ICommand RemoveAttributeCommand { get; private set; }
        public ICommand AddSceneCommand { get; private set; }
        public ICommand RemoveSceneCommand { get; private set; }
        public ICommand AddAssetCommand { get; private set; }
        public ICommand RemoveAssetCommand { get; private set; }
        public ICommand RemoveItemCommand { get; private set; }

        public Game Model
        {
            get { return game; }
        }

        public GameViewModel(Game game)
        {
            this.game = game;

            game.PropertyChanged += Game_PropertyChanged;

            Prototypes = new ComputedObservableCollection<Entity, EntityViewModel>(game.Prototypes, (prototype) => new EntityViewModel(prototype));
            Scenes = new ComputedObservableCollection<Scene, SceneViewModel>(game.Scenes, (scene) => new SceneViewModel(scene));

            AddPrototypeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Entity prototype = new Entity();

                    DialogService.ShowDialog(DialogService.Constants.EntityDialog, prototype,
                        (result) =>
                        {
                            if (result == true)
                            {
                                game.AddPrototype(prototype);
                            }
                        }
                    );
                }
            );

            RemovePrototypeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Entity prototype = parameter as Entity;
                    game.RemovePrototype(prototype);
                }
            );

            AddSceneCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Scene scene = game.CreateScene();

                    DialogService.ShowDialog(DialogService.Constants.SceneDialog, scene,
                        (result) =>
                        {
                            if (result == true)
                            {
                                game.AddScene(scene);
                            }
                        }
                    );
                }
            );

            RemoveSceneCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Scene scene = parameter as Scene;
                    game.RemoveScene(scene);
                }
            );

            AddAttributeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    game.CreateAttribute();
                }
            );

            RemoveAttributeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Attribute attribute = parameter as Attribute;
                    game.RemoveAttribute(attribute);
                }
            );

            AddAssetCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    // TODO: File Chooser
                    Asset asset = new Asset("An Asset");
                    game.AddAsset(asset);
                }
            );

            RemoveAssetCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Asset asset = parameter as Asset;
                    game.RemoveAsset(asset);
                }
            );

            RemoveItemCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Entity prototype = parameter as Entity;
                    if (null != prototype)
                    {
                        game.RemovePrototype(prototype);
                    }
                    else
                    {
                        Scene scene = parameter as Scene;
                        if (null != scene)
                        {
                            game.RemoveScene(scene);
                        }
                    }
                }
            );
        }

        private void Game_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }
    }
}
