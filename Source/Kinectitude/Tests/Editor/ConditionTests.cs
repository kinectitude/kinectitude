using System.Linq;
using Kinectitude.Editor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models.Statements;
using Kinectitude.Editor.Models.Statements.Events;
using Kinectitude.Editor.Models.Statements.Conditions;
using Kinectitude.Editor.Models.Statements.Actions;
using Kinectitude.Editor.Models.Statements.Base;

namespace Kinectitude.Editor.Tests
{
    [TestClass]
    public class ConditionTests
    {
        private static readonly string TriggerOccursEventType = typeof(Kinectitude.Core.Events.TriggerOccursEvent).FullName;
        private static readonly string FireTriggerActionType = typeof(Kinectitude.Core.Actions.FireTriggerAction).FullName;

        [TestMethod]
        public void AddCondition()
        {
            bool collectionChanged = false;

            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            evt.Statements.CollectionChanged += (o, e) => collectionChanged = true;

            Condition condition = new Condition() { If = "test > 1" };
            evt.AddStatement(condition);

            Assert.IsTrue(collectionChanged);
            Assert.AreEqual("test > 1", condition.If);
            Assert.AreEqual(1, evt.Statements.Count);
        }

        [TestMethod]
        public void RemoveCondition()
        {
            int eventsFired = 0;

            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            evt.Statements.CollectionChanged += (o, e) => eventsFired++;

            Condition condition = new Condition() { If = "test > 1" };
            evt.AddStatement(condition);
            evt.RemoveStatement(condition);

            Assert.AreEqual(2, eventsFired);
            Assert.AreEqual(0, evt.Statements.Count);
        }

        [TestMethod]
        public void AddAction()
        {
            bool collectionChanged = false;

            Condition condition = new Condition() { If = "test > 1" };
            condition.Statements.CollectionChanged += (o, e) => collectionChanged = true;

            Action action = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            condition.AddStatement(action);

            Assert.IsTrue(collectionChanged);
            Assert.AreEqual(1, condition.Statements.Count);
        }

        [TestMethod]
        public void RemoveAction()
        {
            int eventsFired = 0;

            Condition condition = new Condition() { If = "test > 1" };
            condition.Statements.CollectionChanged += (o, e) => eventsFired++;

            Action action = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            condition.AddStatement(action);
            condition.RemoveStatement(action);

            Assert.AreEqual(2, eventsFired);
            Assert.AreEqual(0, condition.Statements.Count);
        }

        [TestMethod]
        public void AddReadOnlyCondition()
        {
            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            ReadOnlyEvent childEvent = new ReadOnlyEvent(parentEvent);

            parentEvent.AddStatement(new Condition() { If = "test > 1" });

            Assert.AreEqual(1, childEvent.Statements.Count);

            ReadOnlyCondition childCondition = childEvent.Statements.OfType<ReadOnlyCondition>().Single();

            Assert.IsNotNull(childCondition);
            Assert.AreEqual("test > 1", childCondition.If);
        }

        [TestMethod]
        public void AddInheritedAction()
        {
            Condition parentCondition = new Condition() { If = "test > 1" };
            ReadOnlyCondition childCondition = new ReadOnlyCondition(parentCondition);

            parentCondition.AddStatement(new Action(Workspace.Instance.GetPlugin(FireTriggerActionType)));

            Assert.AreEqual(1, childCondition.Statements.Count);
        }

        [TestMethod]
        public void RemoveInheritedAction()
        {
            Condition parentCondition = new Condition() { If = "test > 1" };
            ReadOnlyCondition childCondition = new ReadOnlyCondition(parentCondition);

            Action parentAction = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentCondition.AddStatement(parentAction);
            parentCondition.RemoveStatement(parentAction);

            Assert.AreEqual(0, childCondition.Statements.Count);
        }

        [TestMethod]
        public void CannotRemoveInheritedActionFromInheritingCondition()
        {
            Condition parentCondition = new Condition() { If = "test > 1" };
            ReadOnlyCondition childCondition = new ReadOnlyCondition(parentCondition);

            Action parentAction = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentCondition.AddStatement(parentAction);

            AbstractStatement childAction = childCondition.Statements.Single();
            childCondition.RemoveStatement(childAction);

            Assert.AreEqual(1, childCondition.Statements.Count);
        }

        [TestMethod]
        public void ReadOnlyConditionFollowsRuleChange()
        {
            bool propertyChanged = false;

            Condition parentCondition = new Condition() { If = "test > 1" };
            
            ReadOnlyCondition childCondition = new ReadOnlyCondition(parentCondition);
            childCondition.PropertyChanged += (o, e) => propertyChanged = (e.PropertyName == "If");

            Assert.AreEqual("test > 1", childCondition.If);

            parentCondition.If = "test <= 1";

            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("test <= 1", childCondition.If);
        }
    }
}
