using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Game
{
    public class SetFirstSceneCommand : IUndoableCommand
    {
        private readonly ICommandHistory history;
        private readonly GameViewModel game;
        private readonly SceneViewModel newScene;
        private readonly SceneViewModel oldScene;

        public string Name
        {
            get { return "Set First Scene"; }
        }

        public SetFirstSceneCommand(ICommandHistory history, GameViewModel game, SceneViewModel newScene)
        {
            this.history = history;
            this.game = game;
            this.newScene = newScene;
            oldScene = game.FirstScene;
        }

        public void Execute()
        {
            game.Game.FirstScene = newScene.Scene;
            game.RaisePropertyChanged("FirstScene");
            history.PushUndo(this);
        }

        public void Unexecute()
        {
            game.Game.FirstScene = oldScene.Scene;
            game.RaisePropertyChanged("FirstScene");
            history.PushRedo(this);
        }
    }
}
