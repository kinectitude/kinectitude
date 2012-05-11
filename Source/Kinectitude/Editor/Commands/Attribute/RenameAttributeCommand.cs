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
        private readonly ICommandHistory history;
        private readonly AttributeViewModel attribute;
        private readonly string newKey;
        private readonly string oldKey;

        public string Name
        {
            get { return string.Format("Rename local attribute to '{0}'", newKey); }
        }

        public RenameAttributeCommand(ICommandHistory history, AttributeViewModel attribute, string newKey)
        {
            this.history = history;
            this.attribute = attribute;
            this.newKey = newKey;
            oldKey = attribute.Key;
        }

        public void Execute()
        {
            if (newKey != oldKey)
            {
                attribute.Attribute.Key = newKey;
                attribute.RaisePropertyChanged("Name");
                history.PushUndo(this);
            }
        }

        public void Unexecute()
        {
            throw new NotImplementedException();
        }
    }
}
