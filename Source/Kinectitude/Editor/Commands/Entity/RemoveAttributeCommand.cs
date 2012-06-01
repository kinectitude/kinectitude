using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Entity
{
    internal sealed class RemoveAttributeCommand : IUndoableCommand
    {
        private readonly EntityViewModel entity;
        private readonly EntityAttributeViewModel attribute;

        public string Name
        {
            get { return string.Format("Remove Attribute '{0}'", attribute.Key); }
        }

        public RemoveAttributeCommand(EntityViewModel entity, EntityAttributeViewModel attribute)
        {
            this.entity = entity;
            this.attribute = attribute;
        }

        public void Execute()
        {
            entity.RemoveAttribute(attribute);
        }

        public void Unexecute()
        {
            entity.AddAttribute(attribute);
        }
    }
}
