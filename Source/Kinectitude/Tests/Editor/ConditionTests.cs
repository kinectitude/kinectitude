using System.Linq;
using Kinectitude.Editor.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models;

namespace Kinectitude.Editor.Tests
{
    [TestClass]
    public class ConditionTests
    {
        private static readonly string TriggerOccursEventType = "Kinectitude.Core.Events.TriggerOccursEvent";
        private static readonly string FireTriggerActionType = "Kinectitude.Core.Actions.FireTriggerAction";

        [TestMethod]
        public void AddCondition()
        {
            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            Condition condition = new Condition() { If = "test > 1" };
            evt.AddAction(condition);

            Assert.AreEqual("test > 1", condition.If);
            Assert.AreEqual(1, evt.Actions.Count);
        }

        [TestMethod]
        public void RemoveCondition()
        {
            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            Condition condition = new Condition() { If = "test > 1" };
            evt.AddAction(condition);
            evt.RemoveAction(condition);

            Assert.AreEqual(0, evt.Actions.Count);
        }

        [TestMethod]
        public void AddAction()
        {
            Condition condition = new Condition() { If = "test > 1" };

            Action action = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            condition.AddAction(action);

            Assert.AreEqual(1, condition.Actions.Count);
        }

        [TestMethod]
        public void RemoveAction()
        {
            Condition condition = new Condition() { If = "test > 1" };

            Action action = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            condition.AddAction(action);
            condition.RemoveAction(action);

            Assert.AreEqual(0, condition.Actions.Count);
        }

        [TestMethod]
        public void AddInheritedCondition()
        {
            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            InheritedEvent childEvent = new InheritedEvent(parentEvent);

            parentEvent.AddAction(new Condition() { If = "test > 1" });

            Assert.AreEqual(1, childEvent.Actions.Count);

            AbstractAction childAction = childEvent.Actions.Single();
            InheritedCondition childCondition = childAction as InheritedCondition;

            Assert.IsNotNull(childCondition);
            Assert.AreEqual("test > 1", childCondition.If);
        }

        [TestMethod]
        public void AddInheritedAction()
        {
            Condition parentCondition = new Condition() { If = "test > 1" };
            InheritedCondition childCondition = new InheritedCondition(parentCondition);

            parentCondition.AddAction(new Action(Workspace.Instance.GetPlugin(FireTriggerActionType)));

            Assert.AreEqual(1, childCondition.Actions.Count);
        }

        [TestMethod]
        public void RemoveInheritedAction()
        {
            Condition parentCondition = new Condition() { If = "test > 1" };
            InheritedCondition childCondition = new InheritedCondition(parentCondition);

            Action parentAction = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentCondition.AddAction(parentAction);
            parentCondition.RemoveAction(parentAction);

            Assert.AreEqual(0, childCondition.Actions.Count);
        }

        /*[TestMethod]
        public void CannotRemoveInheritedActionFromInheritingCondition()
        {
            Condition parentCondition = new Condition() { If = "test > 1" };
            InheritedCondition childCondition = new InheritedCondition(parentCondition);

            Action parentAction = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentCondition.AddAction(parentAction);

            AbstractAction childAction = childCondition.Actions.Single();
            childCondition.RemoveAction();

            Assert.AreEqual(1, childCondition.Actions.Count);
        }*/

        [TestMethod]
        public void InheritedConditionFollowsRuleChange()
        {
            Condition parentCondition = new Condition() { If = "test > 1" };
            InheritedCondition childCondition = new InheritedCondition(parentCondition);

            Assert.AreEqual("test > 1", childCondition.If);

            parentCondition.If = "test <= 1";

            Assert.AreEqual("test <= 1", childCondition.If);
        }
    }
}
