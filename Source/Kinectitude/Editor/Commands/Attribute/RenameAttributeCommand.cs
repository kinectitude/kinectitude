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
        private readonly AttributeViewModel attribute;
        private readonly string newKey;
        private readonly string oldKey;

        public string Name
        {
            get { return string.Format("Rename Attribute to '{0}'", newKey); }
        }

        public RenameAttributeCommand(AttributeViewModel attribute, string newKey)
        {
            this.attribute = attribute;
            this.newKey = newKey;
            oldKey = attribute.Key;
        }

        public void Execute()
        {
            if (newKey != oldKey)
            {
                attribute.Attribute.Key = newKey;
                attribute.FindInheritedAttribute(newKey);
                attribute.RaisePropertyChanged("Key");
                CommandHistory.Instance.PushUndo(this);
            }
        }

        public void Unexecute()
        {
            attribute.Attribute.Key = oldKey;
            attribute.FindInheritedAttribute(oldKey);
            attribute.RaisePropertyChanged("Key");
            CommandHistory.Instance.PushRedo(this);
        }
    }
}
