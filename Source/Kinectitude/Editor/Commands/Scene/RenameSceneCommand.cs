﻿using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Scene
{
    public class RenameSceneCommand : IUndoableCommand
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
            if (newName != oldName)
            {
                scene.Scene.Name = newName;
                scene.RaisePropertyChanged("Name");
                CommandHistory.Instance.PushUndo(this);
            }
        }

        public void Unexecute()
        {
            scene.Scene.Name = oldName;
            scene.RaisePropertyChanged("Name");
            CommandHistory.Instance.PushRedo(this);
        }
    }
}
