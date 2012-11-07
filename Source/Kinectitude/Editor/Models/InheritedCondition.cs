using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Kinectitude.Editor.Models
{
    internal sealed class InheritedCondition : AbstractCondition
    {
        private readonly AbstractCondition inheritedCondition;

        public override string If
        {
            get { return inheritedCondition.If; }
            set { }
        }

        public override Plugin Plugin
        {
            get { return null; }
        }

        public override string Type
        {
            get { return null; }
        }

        public override string DisplayName
        {
            get { return inheritedCondition.DisplayName; }
        }

        public override bool IsLocal
        {
            get { return false; }
        }

        public override bool IsInherited
        {
            get { return true; }
        }

        public override IEnumerable<Plugin> Plugins
        {
            get { return inheritedCondition.Plugins; }
        }

        public InheritedCondition(AbstractCondition inheritedCondition)
        {
            this.inheritedCondition = inheritedCondition;

            inheritedCondition.PropertyChanged += (sender, args) => NotifyPropertyChanged(args.PropertyName);

            inheritedCondition.Actions.CollectionChanged += OnInheritedConditionActionsChanged;
            foreach (AbstractAction inheritedAction in inheritedCondition.Actions)
            {
                InheritAction(inheritedAction);
            }
        }

        public override bool InheritsFrom(AbstractAction action)
        {
            return action == inheritedCondition;
        }

        private void OnInheritedConditionActionsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (AbstractAction inheritedAction in args.NewItems)
                {
                    InheritAction(inheritedAction);
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (AbstractAction inheritedAction in args.OldItems)
                {
                    DisinheritAction(inheritedAction);
                }
            }
        }

        private void InheritAction(AbstractAction inheritedAction)
        {
            AbstractAction localAction = Actions.FirstOrDefault(x => x.InheritsFrom(inheritedAction));
            if (null == localAction)
            {
                AbstractCondition inheritedCondition = inheritedAction as AbstractCondition;
                if (null != inheritedCondition)
                {
                    localAction = new InheritedCondition(inheritedCondition);
                }
                else
                {
                    localAction = new InheritedAction(inheritedAction);
                }

                AddAction(localAction);
            }
        }

        private void DisinheritAction(AbstractAction inheritedAction)
        {
            AbstractAction localAction = Actions.FirstOrDefault(x => x.InheritsFrom(inheritedAction));
            if (null != localAction)
            {
                PrivateRemoveAction(localAction);
            }
        }

        public override AbstractAction DeepCopy()
        {
            Condition copy = new Condition() { If = this.If };

            foreach (AbstractAction action in this.Actions)
            {
                copy.AddAction(action.DeepCopy());
            }

            return copy;
        }
    }
}
