using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Editor.Commands.Base;
using Editor.ViewModels;

namespace Editor.Commands.Game
{
    public class SetDescriptionCommand : IUndoableCommand
    {
        private readonly ICommandHistory history;
        private readonly GameViewModel game;
        private readonly string newDescription;
        private readonly string oldDescription;

        public string Name
        {
            get { return "Change Game Description"; }
        }

        public SetDescriptionCommand(ICommandHistory history, GameViewModel game, string newDescription)
        {
            this.history = history;
            this.game = game;
            this.newDescription = newDescription;
            oldDescription = game.Description;
        }

        public void Execute()
        {
            game.Game.Description = newDescription;
            game.RaisePropertyChanged("Description");
            history.PushUndo(this);
        }

        public void Unexecute()
        {
            game.Game.Description = oldDescription;
            game.RaisePropertyChanged("Description");
            history.PushRedo(this);
        }
    }
}
