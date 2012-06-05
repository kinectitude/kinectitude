using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Property
{
    internal sealed class SetPropertyValueCommand : IUndoableCommand
    {
        private readonly ComponentPropertyViewModel property;
        private readonly object newValue;
        private readonly object oldValue;

        public string Name
        {
            get { return string.Format("Set '{0}' Value", property.Name); }
        }

        public SetPropertyValueCommand(ComponentPropertyViewModel property, object newValue)
        {
            this.property = property;
            this.newValue = newValue;
            this.oldValue = property.Value;
        }

        public void Execute()
        {
            //property.Value = newValue;
        }

        public void Unexecute()
        {
            //property.Value = oldValue;
        }
    }
}
