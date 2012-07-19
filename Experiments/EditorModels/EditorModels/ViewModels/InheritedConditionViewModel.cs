using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace EditorModels.ViewModels
{
    internal sealed class InheritedConditionViewModel : AbstractConditionViewModel
    {
        private readonly AbstractConditionViewModel inheritedCondition;

        public override string If
        {
            get { return inheritedCondition.If; }
            set { }
        }

        public override string Type
        {
            get { return null; }
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
            get { return inheritedCondition.Plugins; }
        }

        public InheritedConditionViewModel(AbstractConditionViewModel inheritedCondition)
        {
            this.inheritedCondition = inheritedCondition;

            inheritedCondition.Actions.CollectionChanged += OnInheritedConditionActionsChanged;
            foreach (AbstractActionViewModel inheritedAction in inheritedCondition.Actions)
            {
                InheritAction(inheritedAction);
            }
        }

        public override bool InheritsFrom(AbstractActionViewModel action)
        {
            return action == inheritedCondition;
        }

        private void OnInheritedConditionActionsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (AbstractActionViewModel inheritedAction in args.NewItems)
                {
                    InheritAction(inheritedAction);
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (AbstractActionViewModel inheritedAction in args.OldItems)
                {
                    DisinheritAction(inheritedAction);
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
    }
}
