using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Editor.ViewModels;
using Editor.Commands.Base;

namespace Editor.Commands
{
    public class SelectEntityCommand : IUndoableCommand
    {
        private readonly ICommandHistory history;
        private readonly SceneViewModel scene;
        private readonly EntityViewModel entity;

        public string Name
        {
            get { return "Select Entity"; }
        }

        public SelectEntityCommand(ICommandHistory history, SceneViewModel scene, EntityViewModel entity)
        {
            this.history = history;
            this.scene = scene;
            this.entity = entity;
        }

        public void Unexecute()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
