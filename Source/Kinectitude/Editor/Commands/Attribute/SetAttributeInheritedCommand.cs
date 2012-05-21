using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Attribute
{
    public class SetAttributeInheritedCommand : IUndoableCommand
    {
        private readonly IAttributeViewModel attribute;
        private readonly bool shouldInherit;
        private readonly bool wasInherited;

        public string Name
        {
            get { return "Change Attribute Inheritance"; }
        }

        public SetAttributeInheritedCommand(IAttributeViewModel attribute, bool shouldInherit)
        {
            this.attribute = attribute;
            this.shouldInherit = shouldInherit;
            wasInherited = attribute.IsInherited;
        }

        public void Execute()
        {
            attribute.IsInherited = shouldInherit;
        }

        public void Unexecute()
        {
            attribute.IsInherited = wasInherited;
        }
    }
}
