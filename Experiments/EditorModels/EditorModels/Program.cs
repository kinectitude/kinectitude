
using EditorModels.ViewModels;
using Kinectitude.Core.Components;
namespace EditorModels
{
    class Program
    {
        static void Main(string[] args)
        {
            GameViewModel game = new GameViewModel("Test Game")
            {
                Width = 800,
                Height = 600,
                IsFullScreen = false
            };

            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            ComponentViewModel parentComponent = new ComponentViewModel(game.GetPlugin(typeof(TransformComponent).FullName));
            PropertyViewModel x = parentComponent.GetProperty("X");
            x.IsInherited = false;
            x.Value = 400;
            parent.AddComponent(parentComponent);
            game.AddPrototype(parent);

            SceneViewModel scene = new SceneViewModel("Test Scene");
            
            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);
            ComponentViewModel childComponent = child.Components[0];
            PropertyViewModel y = childComponent.GetProperty("Y");
            y.IsInherited = false;
            y.Value = 300;
            scene.AddEntity(child);

            game.AddScene(scene);
            game.FirstScene = scene;

            ICommand save = game.SaveGameCommand;
            save.Execute("C:\\Users\\Brandon\\Desktop\\Test Game.xml");
        }
    }
}
