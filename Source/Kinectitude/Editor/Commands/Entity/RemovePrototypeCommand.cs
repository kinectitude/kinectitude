using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Entity
{
    public class RemovePrototypeCommand : IUndoableCommand
    {
        private readonly ICommandHistory history;
        private readonly EntityViewModel entity;
        private readonly EntityViewModel prototype;

        public string Name
        {
            get { return string.Format("Remove Prototype '{0}'", prototype.Name); }
        }

        public RemovePrototypeCommand(ICommandHistory history, EntityViewModel entity, EntityViewModel prototype)
        {
            this.history = history;
            this.entity = entity;
            this.prototype = prototype;
        }

        public void Execute()
        {
            entity.RemovePrototype(prototype);
            history.PushUndo(this);
        }

        public void Unexecute()
        {
            entity.AddPrototype(prototype);
            history.PushRedo(this);
        }
    }
}
