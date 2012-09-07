using System.Linq;
using Kinectitude.Editor.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models;

namespace Kinectitude.Editor.Tests
{
    [TestClass]
    public class ActionTests
    {
        private static readonly string TriggerOccursEventType = "Kinectitude.Core.Events.TriggerOccursEvent";
        private static readonly string FireTriggerActionType = "Kinectitude.Core.Actions.FireTriggerAction";

        [TestMethod]
        public void AddLocalAction()
        {
            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            Action action = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            evt.AddAction(action);

            Assert.IsTrue(action.IsLocal);
            Assert.AreEqual(1, evt.Actions.Count);
        }

        [TestMethod]
        public void RemoveLocalAction()
        {
            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            Action action = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            evt.AddAction(action);
            evt.RemoveAction(action);

            Assert.AreEqual(0, evt.Actions.Count);
        }

        [TestMethod]
        public void AddMultipleActionsOfSameType()
        {
            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            evt.AddAction(new Action(Workspace.Instance.GetPlugin(FireTriggerActionType)));
            evt.AddAction(new Action(Workspace.Instance.GetPlugin(FireTriggerActionType)));

            Assert.AreEqual(2, evt.Actions.Count);
        }

        [TestMethod]
        public void SetActionProperty()
        {
            Action action = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            action.SetProperty("Name", "test");

            Assert.AreEqual("test", action.GetProperty("Name").Value);
        }

        [TestMethod]
        public void AddInheritedAction()
        {
            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parentEvent.AddAction(new Action(Workspace.Instance.GetPlugin(FireTriggerActionType)));

            InheritedEvent childEvent = new InheritedEvent(parentEvent);

            Assert.AreEqual(1, childEvent.Actions.Count);
        }

        [TestMethod]
        public void RemoveInheritedAction()
        {
            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            Action parentAction = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentEvent.AddAction(parentAction);

            InheritedEvent childEvent = new InheritedEvent(parentEvent);

            Assert.AreEqual(1, childEvent.Actions.Count);

            parentEvent.RemoveAction(parentAction);

            Assert.AreEqual(0, childEvent.Actions.Count);
        }

        [TestMethod]
        public void CannotSetPropertyForInheritedAction()
        {
            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            
            Action parentAction = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentAction.SetProperty("Name", "test");
            parentEvent.AddAction(parentAction);

            InheritedEvent childEvent = new InheritedEvent(parentEvent);

            AbstractProperty childProperty = childEvent.Actions.Single().GetProperty("Name");
            childProperty.Value = "test2";

            Assert.AreEqual("test", childProperty.Value);
        }

        [TestMethod]
        public void InheritedActionPropertyFollowsSource()
        {
            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            Action parentAction = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentAction.SetProperty("Name", "test");
            parentEvent.AddAction(parentAction);

            InheritedEvent childEvent = new InheritedEvent(parentEvent);

            AbstractProperty childProperty = childEvent.Actions.Single().GetProperty("Name");
            
            parentAction.SetProperty("Name", "test2");

            Assert.AreEqual("test2", childProperty.Value);
        }

        /*[TestMethod]
        public void CannotRemoveInheritedActionFromInheritingEvent()
        {
            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            Action parentAction = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentEvent.AddAction(parentAction);

            InheritedEvent childEvent = new InheritedEvent(parentEvent);

            Assert.AreEqual(1, childEvent.Actions.Count);

            AbstractAction childAction = childEvent.Actions.Single();
            childEvent.RemoveAction(childAction);

            Assert.AreEqual(0, childEvent.Actions.Count);
        }*/
    }
}
