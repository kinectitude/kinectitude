using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Game
{
    public class SetResolutionCommand : IUndoableCommand
    {
        private readonly ICommandHistory history;
        private readonly GameViewModel game;
        private readonly int oldWidth;
        private readonly int oldHeight;
        private readonly int newWidth;
        private readonly int newHeight;

        public string Name
        {
            get { return "Change Game Resolution"; }
        }

        public SetResolutionCommand(ICommandHistory history, GameViewModel game, int newWidth, int newHeight)
        {
            this.history = history;
            this.game = game;
            this.newWidth = newWidth;
            this.newHeight = newHeight;
            oldWidth = game.Width;
            oldHeight = game.Height;
        }

        public void Execute()
        {
            game.Game.Width = newWidth;
            game.Game.Height = newHeight;
            game.RaisePropertyChanged("Width");
            game.RaisePropertyChanged("Height");
            history.PushUndo(this);
        }

        public void Unexecute()
        {
            game.Game.Width = oldWidth;
            game.Game.Height = oldHeight;
            game.RaisePropertyChanged("Width");
            game.RaisePropertyChanged("Height");
            history.PushRedo(this);
        }
    }
}
