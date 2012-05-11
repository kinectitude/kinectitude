﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Game
{
    public class SetFullScreenCommand : IUndoableCommand
    {
        private readonly GameViewModel game;
        private readonly bool newValue;
        private readonly bool oldValue;

        public string Name
        {
            get { return "Set Game Full Screen"; }
        }

        public SetFullScreenCommand(GameViewModel game, bool newValue)
        {
            this.game = game;
            this.newValue = newValue;
            oldValue = game.IsFullScreen;
        }

        public void Execute()
        {
            game.Game.IsFullScreen = newValue;
            game.RaisePropertyChanged("IsFullScreen");
            CommandHistory.Instance.PushUndo(this);
        }

        public void Unexecute()
        {
            game.Game.IsFullScreen = oldValue;
            game.RaisePropertyChanged("IsFullScreen");
            CommandHistory.Instance.PushRedo(this);
        }
    }
}
