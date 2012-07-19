using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EditorModels.ViewModels;

namespace EditorModels.Tests
{
    [TestClass]
    public class ActionViewModelTests
    {
        private static readonly string TriggerOccursEventType = "Kinectitude.Core.Events.TriggerOccursEvent";
        private static readonly string FireTriggerActionType = "Kinectitude.Core.Actions.FireTriggerAction";

        [TestMethod]
        public void AddLocalAction()
        {
            EventViewModel evt = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            ActionViewModel action = new ActionViewModel(Workspace.Instance.GetPlugin(FireTriggerActionType));
            evt.AddAction(action);

            Assert.IsTrue(action.IsLocal);
            Assert.AreEqual(1, evt.Actions.Count);
        }

        [TestMethod]
        public void RemoveLocalAction()
        {
            EventViewModel evt = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            ActionViewModel action = new ActionViewModel(Workspace.Instance.GetPlugin(FireTriggerActionType));
            evt.AddAction(action);
            evt.RemoveAction(action);

            Assert.AreEqual(0, evt.Actions.Count);
        }

        [TestMethod]
        public void AddMultipleActionsOfSameType()
        {
            EventViewModel evt = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            evt.AddAction(new ActionViewModel(Workspace.Instance.GetPlugin(FireTriggerActionType)));
            evt.AddAction(new ActionViewModel(Workspace.Instance.GetPlugin(FireTriggerActionType)));

            Assert.AreEqual(2, evt.Actions.Count);
        }

        [TestMethod]
        public void SetActionProperty()
        {
            ActionViewModel action = new ActionViewModel(Workspace.Instance.GetPlugin(FireTriggerActionType));
            action.SetProperty("Name", "test");

            Assert.AreEqual("test", action.GetProperty("Name").Value);
        }

        [TestMethod]
        public void AddInheritedAction()
        {
            EventViewModel parentEvent = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parentEvent.AddAction(new ActionViewModel(Workspace.Instance.GetPlugin(FireTriggerActionType)));

            InheritedEventViewModel childEvent = new InheritedEventViewModel(parentEvent);

            Assert.AreEqual(1, childEvent.Actions.Count);
        }

        [TestMethod]
        public void RemoveInheritedAction()
        {
            EventViewModel parentEvent = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            ActionViewModel parentAction = new ActionViewModel(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentEvent.AddAction(parentAction);

            InheritedEventViewModel childEvent = new InheritedEventViewModel(parentEvent);

            Assert.AreEqual(1, childEvent.Actions.Count);

            parentEvent.RemoveAction(parentAction);

            Assert.AreEqual(0, childEvent.Actions.Count);
        }

        [TestMethod]
        public void CannotSetPropertyForInheritedAction()
        {
            EventViewModel parentEvent = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            
            ActionViewModel parentAction = new ActionViewModel(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentAction.SetProperty("Name", "test");
            parentEvent.AddAction(parentAction);

            InheritedEventViewModel childEvent = new InheritedEventViewModel(parentEvent);

            AbstractPropertyViewModel childProperty = childEvent.Actions.Single().GetProperty("Name");
            childProperty.Value = "test2";

            Assert.AreEqual("test", childProperty.Value);
        }

        [TestMethod]
        public void InheritedActionPropertyFollowsSource()
        {
            EventViewModel parentEvent = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            ActionViewModel parentAction = new ActionViewModel(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentAction.SetProperty("Name", "test");
            parentEvent.AddAction(parentAction);

            InheritedEventViewModel childEvent = new InheritedEventViewModel(parentEvent);

            AbstractPropertyViewModel childProperty = childEvent.Actions.Single().GetProperty("Name");
            
            parentAction.SetProperty("Name", "test2");

            Assert.AreEqual("test2", childProperty.Value);
        }

        /*[TestMethod]
        public void CannotRemoveInheritedActionFromInheritingEvent()
        {
            EventViewModel parentEvent = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            ActionViewModel parentAction = new ActionViewModel(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentEvent.AddAction(parentAction);

            InheritedEventViewModel childEvent = new InheritedEventViewModel(parentEvent);

            Assert.AreEqual(1, childEvent.Actions.Count);

            AbstractActionViewModel childAction = childEvent.Actions.Single();
            childEvent.RemoveAction(childAction);

            Assert.AreEqual(0, childEvent.Actions.Count);
        }*/
    }
}
