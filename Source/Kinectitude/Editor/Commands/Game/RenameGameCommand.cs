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
            game.Name = newName;
        }

        public void Unexecute()
        {
            game.Name = oldName;
        }
    }
}
