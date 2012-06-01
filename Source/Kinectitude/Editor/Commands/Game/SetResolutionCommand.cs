using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Game
{
    internal sealed class SetResolutionCommand : IUndoableCommand
    {
        private readonly GameViewModel game;
        private readonly int oldWidth;
        private readonly int oldHeight;
        private readonly int newWidth;
        private readonly int newHeight;

        public string Name
        {
            get { return "Change Game Resolution"; }
        }

        public SetResolutionCommand(GameViewModel game, int newWidth, int newHeight)
        {
            this.game = game;
            this.newWidth = newWidth;
            this.newHeight = newHeight;
            oldWidth = game.Width;
            oldHeight = game.Height;
        }

        public void Execute()
        {
            game.Width = newWidth;
            game.Height = newHeight;
        }

        public void Unexecute()
        {
            game.Width = oldWidth;
            game.Height = oldHeight;
        }
    }
}
