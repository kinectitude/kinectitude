using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Game
{
    internal sealed class DeleteSceneCommand : IUndoableCommand
    {
        private readonly GameViewModel game;
        private readonly SceneViewModel scene;

        public string Name
        {
            get { return string.Format("Delete Scene '{0}'", scene.Name); }
        }

        public DeleteSceneCommand(GameViewModel game, SceneViewModel scene)
        {
            this.game = game;
            this.scene = scene;
        }

        public void Unexecute()
        {
            if (null != scene)
            {
                game.AddScene(scene);
            }
        }

        public void Execute()
        {
            if (null != scene)
            {
                game.RemoveScene(scene);
            }
        }
    }
}
