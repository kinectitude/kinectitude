using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Entity
{
    internal sealed class AddPrototypeCommand : IUndoableCommand
    {
        private readonly EntityViewModel entity;
        private readonly EntityViewModel prototype;

        public string Name
        {
            get { return string.Format("Add Prototype '{0}'", prototype.Name); }
        }

        public AddPrototypeCommand(EntityViewModel entity, EntityViewModel prototype)
        {
            this.entity = entity;
            this.prototype = prototype;
        }

        public void Execute()
        {
            entity.AddPrototype(prototype);
        }

        public void Unexecute()
        {
            entity.RemovePrototype(prototype);
        }
    }
}
