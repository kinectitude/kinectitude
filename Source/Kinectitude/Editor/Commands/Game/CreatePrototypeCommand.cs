﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Editor.Commands.Base;
using Editor.ViewModels;

namespace Editor.Commands.Game
{
    public class CreatePrototypeCommand : IUndoableCommand
    {
        private readonly ICommandHistory history;
        private readonly GameViewModel game;
        private readonly EntityViewModel prototype;

        public string Name
        {
            get { return string.Format("Create Prototype '{0}'", prototype.Name); }
        }

        public CreatePrototypeCommand(ICommandHistory history, GameViewModel game, EntityViewModel prototype)
        {
            this.history = history;
            this.game = game;
            this.prototype = prototype;
        }

        public void Execute()
        {
            if (null != prototype)
            {
                game.AddPrototype(prototype);
                history.PushUndo(this);
            }
        }

        public void Unexecute()
        {
            if (null != prototype)
            {
                game.RemovePrototype(prototype);
                history.PushRedo(this);
            }
        }
    }
}
