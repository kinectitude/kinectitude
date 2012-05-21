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
        private readonly IAttributeViewModel attribute;
        private readonly string newValue;
        private readonly string oldValue;

        public string Name
        {
            get { return "Set Attribute Value"; }
        }

        public SetAttributeValueCommand(IAttributeViewModel attribute, string newValue)
        {
            this.attribute = attribute;
            this.newValue = newValue;
            oldValue = attribute.Value.ToString();
        }

        public void Execute()
        {
            attribute.Value = newValue;
        }

        public void Unexecute()
        {
            attribute.Value = oldValue;
        }
    }
}
