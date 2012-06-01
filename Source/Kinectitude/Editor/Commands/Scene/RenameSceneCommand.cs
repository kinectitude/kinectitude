using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Scene
{
    internal sealed class RenameSceneCommand : IUndoableCommand
    {
        private readonly SceneViewModel scene;
        private readonly string newName;
        private readonly string oldName;

        public string Name
        {
            get { return "Rename Scene"; }
        }

        public RenameSceneCommand(SceneViewModel scene, string newName)
        {
            this.scene = scene;
            this.newName = newName;
            oldName = scene.Name;
        }

        public void Execute()
        {
            scene.Name = newName;
        }

        public void Unexecute()
        {
            scene.Name = oldName;
        }
    }
}
