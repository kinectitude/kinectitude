using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Entity
{
    public class RemoveAttributeCommand : IUndoableCommand
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
