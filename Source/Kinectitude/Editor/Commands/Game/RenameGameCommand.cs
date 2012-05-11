using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Game
{
    public class RenameGameCommand : IUndoableCommand
    {
        private readonly GameViewModel game;
        private readonly string newName;
        private readonly string oldName;

        public string Name
        {
            get { return "Rename Game"; }
        }

        public RenameGameCommand(GameViewModel game, string newName)
        {
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
                CommandHistory.Instance.PushUndo(this);
            }
        }

        public void Unexecute()
        {
            game.Game.Name = oldName;
            game.RaisePropertyChanged("Name");
            CommandHistory.Instance.PushRedo(this);
        }
    }
}
