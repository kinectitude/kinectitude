using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Editor.Commands.Base;
using Editor.ViewModels;

namespace Editor.Commands.Game
{
    public class SetFullScreenCommand : IUndoableCommand
    {
        private readonly ICommandHistory history;
        private readonly GameViewModel game;
        private readonly bool newValue;
        private readonly bool oldValue;

        public string Name
        {
            get { return "Set Game Full Screen"; }
        }

        public SetFullScreenCommand(ICommandHistory history, GameViewModel game, bool newValue)
        {
            this.history = history;
            this.game = game;
            this.newValue = newValue;
            oldValue = game.IsFullScreen;
        }

        public void Execute()
        {
            game.Game.IsFullScreen = newValue;
            game.RaisePropertyChanged("IsFullScreen");
            history.PushUndo(this);
        }

        public void Unexecute()
        {
            game.Game.IsFullScreen = oldValue;
            game.RaisePropertyChanged("IsFullScreen");
            history.PushRedo(this);
        }
    }
}
