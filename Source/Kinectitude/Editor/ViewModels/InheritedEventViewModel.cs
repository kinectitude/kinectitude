using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class InheritedEventViewModel : AbstractEventViewModel
    {
        private readonly AbstractEventViewModel inheritedEvent;

        public override event DefineAddedEventHandler DefineAdded
        {
            add { inheritedEvent.DefineAdded += value; }
            remove { inheritedEvent.DefineAdded -= value; }
        }

        public override event DefinedNameChangedEventHandler DefineChanged
        {
            add { inheritedEvent.DefineChanged += value; }
            remove { inheritedEvent.DefineChanged -= value; }
        }

        public override string Type
        {
            get { return inheritedEvent.Type; }
        }

        public override bool IsLocal
        {
            get { return false; }
        }

        public override bool IsInherited
        {
            get { return true; }
        }

        public override IEnumerable<PluginViewModel> Plugins
        {
            get { return inheritedEvent.Plugins; }
        }

        public InheritedEventViewModel(AbstractEventViewModel inheritedEvent)
        {
            this.inheritedEvent = inheritedEvent;

            inheritedEvent.Actions.CollectionChanged += OnInheritedEventActionsChanged;
            foreach (AbstractActionViewModel inheritedAction in inheritedEvent.Actions)
            {
                InheritAction(inheritedAction);
            }
            
            inheritedEvent.PropertyChanged += (sender, args) => NotifyPropertyChanged(args.PropertyName);

            foreach (AbstractPropertyViewModel inheritedProperty in inheritedEvent.Properties)
            {
                InheritedPropertyViewModel localProperty = new InheritedPropertyViewModel(inheritedProperty);
                AddProperty(localProperty);
            }
        }

        private void OnInheritedEventActionsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (AbstractActionViewModel action in args.NewItems)
                {
                    InheritAction(action);
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (AbstractActionViewModel action in args.OldItems)
                {
                    DisinheritAction(action);
                }
            }
        }

        private void InheritAction(AbstractActionViewModel inheritedAction)
        {
            AbstractActionViewModel localAction = Actions.FirstOrDefault(x => x.InheritsFrom(inheritedAction));
            if (null == localAction)
            {
                AbstractConditionViewModel inheritedCondition = inheritedAction as AbstractConditionViewModel;
                if (null != inheritedCondition)
                {
                    localAction = new InheritedConditionViewModel(inheritedCondition);
                }
                else
                {
                    localAction = new InheritedActionViewModel(inheritedAction);
                }
                
                AddAction(localAction);
            }
        }

        private void DisinheritAction(AbstractActionViewModel inheritedAction)
        {
            AbstractActionViewModel localAction = Actions.FirstOrDefault(x => x.InheritsFrom(inheritedAction));
            if (null != localAction)
            {
                PrivateRemoveAction(localAction);
            }
        }

        public override bool InheritsFrom(AbstractEventViewModel evt)
        {
            return evt == inheritedEvent;
        }
    }
}
