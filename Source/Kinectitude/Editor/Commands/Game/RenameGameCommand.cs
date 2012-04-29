using Editor.ViewModels;
using Editor.Commands.Base;

namespace Editor.Commands.Game
{
    public class RenameGameCommand : IUndoableCommand
    {
        private readonly ICommandHistory history;
        private readonly GameViewModel game;
        private readonly string newName;
        private readonly string oldName;

        public string Name
        {
            get { return "Rename Game"; }
        }

        public RenameGameCommand(ICommandHistory history, GameViewModel game, string newName)
        {
            this.history = history;
            this.game = game;
            this.newName = newName;
            oldName = game.Name;
        }

        public void Execute()
        {
            if (newName != oldName)
            {
                game.Game.Name = newName;
                game.RaisePropertyChanged("Name");
                history.PushUndo(this);
            }
        }

        public void Unexecute()
        {
            game.Game.Name = oldName;
            game.RaisePropertyChanged("Name");
            history.PushRedo(this);
        }
    }
}
