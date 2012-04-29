using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Editor.ViewModels;
using Editor.Commands.Base;

namespace Editor.Commands.Game
{
    public class DeleteSceneCommand : IUndoableCommand
    {
        private readonly ICommandHistory history;
        private readonly GameViewModel game;
        private readonly SceneViewModel scene;

        public string Name
        {
            get { return string.Format("Delete Scene '{0}'", scene.Name); }
        }

        public DeleteSceneCommand(ICommandHistory history, GameViewModel game, SceneViewModel scene)
        {
            this.history = history;
            this.game = game;
            this.scene = scene;
        }

        public void Unexecute()
        {
            if (null != scene)
            {
                game.AddScene(scene);
                history.PushRedo(this);
            }
        }

        public void Execute()
        {
            if (null != scene)
            {
                game.RemoveScene(scene);
                history.PushUndo(this);
            }
        }
    }
}
