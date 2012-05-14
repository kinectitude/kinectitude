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
        private readonly EntityViewModel entity;
        private readonly EntityViewModel prototype;

        public string Name
        {
            get { return string.Format("Remove Prototype '{0}'", prototype.Name); }
        }

        public RemovePrototypeCommand(EntityViewModel entity, EntityViewModel prototype)
        {
            this.entity = entity;
            this.prototype = prototype;
        }

        public void Execute()
        {
            entity.RemovePrototype(prototype);
        }

        public void Unexecute()
        {
            entity.AddPrototype(prototype);
        }
    }
}
