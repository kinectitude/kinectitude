using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Game
{
    internal sealed class SetDescriptionCommand : IUndoableCommand
    {
        private readonly GameViewModel game;
        private readonly string newDescription;
        private readonly string oldDescription;

        public string Name
        {
            get { return "Change Game Description"; }
        }

        public SetDescriptionCommand(GameViewModel game, string newDescription)
        {
            this.game = game;
            this.newDescription = newDescription;
            oldDescription = game.Description;
        }

        public void Execute()
        {
            game.Description = newDescription;
        }

        public void Unexecute()
        {
            game.Description = oldDescription;
        }
    }
}
