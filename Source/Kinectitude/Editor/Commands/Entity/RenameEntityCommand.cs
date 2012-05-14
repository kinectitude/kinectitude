using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Entity
{
    public class RenameEntityCommand : IUndoableCommand
    {
        private readonly EntityViewModel entity;
        private readonly string newName;
        private readonly string oldName;

        public string Name
        {
            get { return "Rename Entity"; }
        }

        public RenameEntityCommand(EntityViewModel entity, string newName)
        {
            this.entity = entity;
            this.newName = newName;
            oldName = entity.Name;
        }

        public void Execute()
        {
            entity.Name = newName;
        }

        public void Unexecute()
        {
            entity.Name = oldName;
        }
    }
}
