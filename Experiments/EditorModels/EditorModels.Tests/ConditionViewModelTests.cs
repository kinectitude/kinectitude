using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EditorModels.ViewModels;

namespace EditorModels.Tests
{
    [TestClass]
    public class ConditionViewModelTests
    {
        private static readonly string TriggerOccursEventType = "Kinectitude.Core.Events.TriggerOccursEvent";
        private static readonly string FireTriggerActionType = "Kinectitude.Core.Actions.FireTriggerAction";

        [TestMethod]
        public void AddCondition()
        {
            EventViewModel evt = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            ConditionViewModel condition = new ConditionViewModel() { If = "test > 1" };
            evt.AddAction(condition);

            Assert.AreEqual("test > 1", condition.If);
            Assert.AreEqual(1, evt.Actions.Count);
        }

        [TestMethod]
        public void RemoveCondition()
        {
            EventViewModel evt = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            ConditionViewModel condition = new ConditionViewModel() { If = "test > 1" };
            evt.AddAction(condition);
            evt.RemoveAction(condition);

            Assert.AreEqual(0, evt.Actions.Count);
        }

        [TestMethod]
        public void AddAction()
        {
            ConditionViewModel condition = new ConditionViewModel() { If = "test > 1" };

            ActionViewModel action = new ActionViewModel(Workspace.Instance.GetPlugin(FireTriggerActionType));
            condition.AddAction(action);

            Assert.AreEqual(1, condition.Actions.Count);
        }

        [TestMethod]
        public void RemoveAction()
        {
            ConditionViewModel condition = new ConditionViewModel() { If = "test > 1" };

            ActionViewModel action = new ActionViewModel(Workspace.Instance.GetPlugin(FireTriggerActionType));
            condition.AddAction(action);
            condition.RemoveAction(action);

            Assert.AreEqual(0, condition.Actions.Count);
        }

        [TestMethod]
        public void AddInheritedCondition()
        {
            EventViewModel parentEvent = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            InheritedEventViewModel childEvent = new InheritedEventViewModel(parentEvent);

            parentEvent.AddAction(new ConditionViewModel() { If = "test > 1" });

            Assert.AreEqual(1, childEvent.Actions.Count);

            AbstractActionViewModel childAction = childEvent.Actions.Single();
            InheritedConditionViewModel childCondition = childAction as InheritedConditionViewModel;

            Assert.IsNotNull(childCondition);
            Assert.AreEqual("test > 1", childCondition.If);
        }

        [TestMethod]
        public void AddInheritedAction()
        {
            ConditionViewModel parentCondition = new ConditionViewModel() { If = "test > 1" };
            InheritedConditionViewModel childCondition = new InheritedConditionViewModel(parentCondition);

            parentCondition.AddAction(new ActionViewModel(Workspace.Instance.GetPlugin(FireTriggerActionType)));

            Assert.AreEqual(1, childCondition.Actions.Count);
        }

        [TestMethod]
        public void RemoveInheritedAction()
        {
            ConditionViewModel parentCondition = new ConditionViewModel() { If = "test > 1" };
            InheritedConditionViewModel childCondition = new InheritedConditionViewModel(parentCondition);

            ActionViewModel parentAction = new ActionViewModel(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentCondition.AddAction(parentAction);
            parentCondition.RemoveAction(parentAction);

            Assert.AreEqual(0, childCondition.Actions.Count);
        }

        /*[TestMethod]
        public void CannotRemoveInheritedActionFromInheritingCondition()
        {
            ConditionViewModel parentCondition = new ConditionViewModel() { If = "test > 1" };
            InheritedConditionViewModel childCondition = new InheritedConditionViewModel(parentCondition);

            ActionViewModel parentAction = new ActionViewModel(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentCondition.AddAction(parentAction);

            AbstractActionViewModel childAction = childCondition.Actions.Single();
            childCondition.RemoveAction();

            Assert.AreEqual(1, childCondition.Actions.Count);
        }*/

        [TestMethod]
        public void InheritedConditionFollowsRuleChange()
        {
            ConditionViewModel parentCondition = new ConditionViewModel() { If = "test > 1" };
            InheritedConditionViewModel childCondition = new InheritedConditionViewModel(parentCondition);

            Assert.AreEqual("test > 1", childCondition.If);

            parentCondition.If = "test <= 1";

            Assert.AreEqual("test <= 1", childCondition.If);
        }
    }
}
