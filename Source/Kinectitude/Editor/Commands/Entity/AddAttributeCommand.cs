using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Entity
{
    internal sealed class AddAttributeCommand : IUndoableCommand
    {
        private readonly EntityViewModel entity;
        private readonly EntityAttributeViewModel attribute;

        public string Name
        {
            get { return string.Format("Add Attribute '{0}'", attribute.Key); }
        }

        public AddAttributeCommand(EntityViewModel entity, EntityAttributeViewModel attribute)
        {
            this.entity = entity;
            this.attribute = attribute;
        }

        public void Execute()
        {
            entity.AddAttribute(attribute);
        }

        public void Unexecute()
        {
            entity.RemoveAttribute(attribute);
        }
    }
}
