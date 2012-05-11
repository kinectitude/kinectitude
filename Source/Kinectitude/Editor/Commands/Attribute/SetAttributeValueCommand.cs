using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Attribute
{
    public class SetAttributeValueCommand : IUndoableCommand
    {
        private readonly AttributeViewModel attribute;
        private readonly dynamic newValue;
        private readonly dynamic oldValue;

        public string Name
        {
            get { return "Set Attribute Value"; }
        }

        public SetAttributeValueCommand(AttributeViewModel attribute, string newValue)
        {
            this.attribute = attribute;
            this.newValue = Kinectitude.Editor.Models.Base.Attribute.TryParse(newValue);
            oldValue = attribute.Value;
        }

        public void Execute()
        {
            attribute.Attribute.Value = newValue;
            attribute.RaisePropertyChanged("Value");
            CommandHistory.Instance.PushUndo(this);
        }

        public void Unexecute()
        {
            attribute.Attribute.Value = oldValue;
            attribute.RaisePropertyChanged("Value");
            CommandHistory.Instance.PushRedo(this);
        }
    }
}
