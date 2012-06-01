using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Attribute
{
    internal sealed class SetAttributeInheritedCommand : IUndoableCommand
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
