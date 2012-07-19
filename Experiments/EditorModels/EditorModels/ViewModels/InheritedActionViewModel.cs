using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.ViewModels
{
    internal sealed class InheritedActionViewModel : AbstractActionViewModel
    {
        private readonly AbstractActionViewModel inheritedAction;

        public override string Type
        {
            get { return inheritedAction.Type; }
        }

        public override bool IsLocal
        {
            get { return inheritedAction.IsLocal; }
        }

        public override bool IsInherited
        {
            get { return inheritedAction.IsInherited; }
        }

        public override IEnumerable<PluginViewModel> Plugins
        {
            get { return inheritedAction.Plugins; }
        }

        public InheritedActionViewModel(AbstractActionViewModel inheritedAction)
        {
            this.inheritedAction = inheritedAction;

            inheritedAction.PropertyChanged += (sender, args) => NotifyPropertyChanged(args.PropertyName);

            foreach (AbstractPropertyViewModel inheritedProperty in inheritedAction.Properties)
            {
                InheritedPropertyViewModel localProperty = new InheritedPropertyViewModel(inheritedProperty);
                AddProperty(localProperty);
            }
        }

        public override bool InheritsFrom(AbstractActionViewModel action)
        {
            return action == inheritedAction;
        }
    }
}
