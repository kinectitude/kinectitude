using System.Linq;
using Kinectitude.Editor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Action = Kinectitude.Editor.Models.Statements.Actions.Action;
using Kinectitude.Editor.Models.Statements;
using Kinectitude.Editor.Models.Statements.Events;
using Kinectitude.Editor.Models.Statements.Actions;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Statements.Base;
using Kinectitude.Editor.Models.Values;

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
            int eventsRaised = 0;

            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            evt.Statements.CollectionChanged += (sender, e) => eventsRaised++;

            Action action = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            evt.AddStatement(action);

            Assert.AreEqual(1, eventsRaised);
            Assert.IsTrue(action.IsLocal);
            Assert.AreEqual(1, evt.Statements.Count);
        }

        [TestMethod]
        public void RemoveLocalAction()
        {
            int eventsRaised = 0;

            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            evt.Statements.CollectionChanged += (sender, e) => eventsRaised++;

            Action action = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            evt.AddStatement(action);
            evt.RemoveStatement(action);

            Assert.AreEqual(2, eventsRaised);
            Assert.AreEqual(0, evt.Statements.Count);
        }

        [TestMethod]
        public void AddMultipleActionsOfSameType()
        {
            int eventsRaised = 0;

            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            evt.Statements.CollectionChanged += (sender, e) => eventsRaised++;

            evt.AddStatement(new Action(Workspace.Instance.GetPlugin(FireTriggerActionType)));
            evt.AddStatement(new Action(Workspace.Instance.GetPlugin(FireTriggerActionType)));

            Assert.AreEqual(2, eventsRaised);
            Assert.AreEqual(2, evt.Statements.Count);
        }

        [TestMethod]
        public void SetActionProperty()
        {
            bool propertyFiredValueChange = false;

            Action action = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));

            action.GetProperty("Name").PropertyChanged += (sender, e) => propertyFiredValueChange |= (e.PropertyName == "Value");

            action.SetProperty("Name", new Value("test"));

            Assert.IsTrue(propertyFiredValueChange);
            Assert.AreEqual("test", action.GetProperty("Name").Value);
        }

        [TestMethod]
        public void AddInheritedAction()
        {
            int parentEventsRaised = 0;
            int childEventsRaised = 0;

            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parentEvent.Statements.CollectionChanged += (sender, e) => parentEventsRaised++;

            parentEvent.AddStatement(new Action(Workspace.Instance.GetPlugin(FireTriggerActionType)));

            ReadOnlyEvent childEvent = new ReadOnlyEvent(parentEvent);
            childEvent.Statements.CollectionChanged += (sender, e) => childEventsRaised++;

            Assert.AreEqual(1, childEvent.Statements.Count);

            parentEvent.AddStatement(new Action(Workspace.Instance.GetPlugin(FireTriggerActionType)));

            Assert.AreEqual(2, childEvent.Statements.Count);
            Assert.AreEqual(2, parentEventsRaised);
            Assert.AreEqual(1, childEventsRaised);
        }

        [TestMethod]
        public void RemoveInheritedAction()
        {
            int parentEventsRaised = 0;
            int childEventsRaised = 0;

            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parentEvent.Statements.CollectionChanged += (sender, e) => parentEventsRaised++;

            Action parentAction = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentEvent.AddStatement(parentAction);

            ReadOnlyEvent childEvent = new ReadOnlyEvent(parentEvent);
            childEvent.Statements.CollectionChanged += (sender, e) => childEventsRaised++;

            Assert.AreEqual(1, childEvent.Statements.Count);

            parentEvent.RemoveStatement(parentAction);

            Assert.AreEqual(0, childEvent.Statements.Count);
            Assert.AreEqual(2, parentEventsRaised);
            Assert.AreEqual(1, childEventsRaised);
        }

        [TestMethod]
        public void CannotSetPropertyForInheritedAction()
        {
            int parentEventsRaised = 0;
            bool parentPropertyChanged = false;
            bool childPropertyChanged = false;

            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parentEvent.Statements.CollectionChanged += (sender, e) => parentEventsRaised++;

            Action parentAction = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));

            parentAction.GetProperty("Name").PropertyChanged += (sender, e) => parentPropertyChanged |= (e.PropertyName == "Value");

            parentAction.SetProperty("Name", new Value("test"));
            parentEvent.AddStatement(parentAction);

            ReadOnlyEvent childEvent = new ReadOnlyEvent(parentEvent);

            AbstractProperty childProperty = ((AbstractAction)childEvent.Statements.Single()).GetProperty("Name");
            childProperty.PropertyChanged += (sender, e) => childPropertyChanged |= (e.PropertyName == "Value");

            childProperty.Value = new Value("test2");

            Assert.AreEqual("test", childProperty.Value);
            Assert.AreEqual(1, parentEventsRaised);
            Assert.IsTrue(parentPropertyChanged);
            Assert.IsFalse(childPropertyChanged);
        }

        [TestMethod]
        public void InheritedActionPropertyFollowsSource()
        {
            bool childPropertyChanged = false;

            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            Action parentAction = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentAction.SetProperty("Name", new Value("test"));
            parentEvent.AddStatement(parentAction);

            ReadOnlyEvent childEvent = new ReadOnlyEvent(parentEvent);

            AbstractProperty childProperty = ((AbstractAction)childEvent.Statements.Single()).GetProperty("Name");
            childProperty.PropertyChanged += (sender, e) => childPropertyChanged |= (e.PropertyName == "Value");
            
            parentAction.SetProperty("Name", new Value("test2"));

            Assert.AreEqual("test2", childProperty.Value);
            Assert.IsTrue(childPropertyChanged);
        }

        [TestMethod]
        public void CannotRemoveInheritedActionFromInheritingEvent()
        {
            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            Action parentAction = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentEvent.AddStatement(parentAction);

            ReadOnlyEvent childEvent = new ReadOnlyEvent(parentEvent);

            Assert.AreEqual(1, childEvent.Statements.Count);

            AbstractStatement childAction = childEvent.Statements.Single();
            childEvent.RemoveStatement(childAction);

            Assert.AreEqual(1, childEvent.Statements.Count);
        }
    }
}
