using System.Collections.Generic;

namespace Kinectitude.Editor.Models
{
    internal sealed class InheritedAction : AbstractAction
    {
        private readonly AbstractAction inheritedAction;

        public override string Type
        {
            get { return inheritedAction.Type; }
        }

        public override string DisplayName
        {
            get { return inheritedAction.DisplayName; }
        }

        public override bool IsLocal
        {
            get { return inheritedAction.IsLocal; }
        }

        public override bool IsInherited
        {
            get { return inheritedAction.IsInherited; }
        }

        public override IEnumerable<Plugin> Plugins
        {
            get { return inheritedAction.Plugins; }
        }

        public InheritedAction(AbstractAction inheritedAction)
        {
            this.inheritedAction = inheritedAction;

            inheritedAction.PropertyChanged += (sender, args) => NotifyPropertyChanged(args.PropertyName);

            foreach (AbstractProperty inheritedProperty in inheritedAction.Properties)
            {
                InheritedProperty localProperty = new InheritedProperty(inheritedProperty);
                AddProperty(localProperty);
            }
        }

        public override bool InheritsFrom(AbstractAction action)
        {
            return action == inheritedAction;
        }
    }
}
