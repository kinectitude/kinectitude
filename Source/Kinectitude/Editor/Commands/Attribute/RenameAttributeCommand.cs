using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Attribute
{
    public class RenameAttributeCommand : IUndoableCommand
    {
        private readonly IAttributeViewModel attribute;
        private readonly string newKey;
        private readonly string oldKey;

        public string Name
        {
            get { return string.Format("Rename Attribute to '{0}'", newKey); }
        }

        public RenameAttributeCommand(IAttributeViewModel attribute, string newKey)
        {
            this.attribute = attribute;
            this.newKey = newKey;
            oldKey = attribute.Key;
        }

        public void Execute()
        {
            attribute.Key = newKey;
        }

        public void Unexecute()
        {
            attribute.Key = oldKey;
        }
    }
}
