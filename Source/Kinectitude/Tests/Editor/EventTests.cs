using System.Linq;
using Kinectitude.Editor.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models;

namespace Kinectitude.Editor.Tests
{
    [TestClass]
    public class EventTests
    {
        private static readonly string TriggerOccursEventType = "Kinectitude.Core.Events.TriggerOccursEvent";

        [TestMethod]
        public void AddLocalEvent()
        {
            Entity entity = new Entity();

            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            entity.AddEvent(evt);

            Assert.IsTrue(evt.IsLocal);
            Assert.AreEqual(1, entity.Events.Count);
        }

        [TestMethod]
        public void RemoveLocalEvent()
        {
            Entity entity = new Entity();

            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            entity.AddEvent(evt);
            entity.RemoveEvent(evt);

            Assert.AreEqual(0, entity.Events.Count);
        }

        [TestMethod]
        public void AddMultipleEventsOfSameType()
        {
            Entity entity = new Entity();

            entity.AddEvent(new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType)));
            entity.AddEvent(new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType)));

            Assert.AreEqual(2, entity.Events.Count);
        }

        [TestMethod]
        public void SetEventProperty()
        {
            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            evt.SetProperty("Trigger", "test");

            Assert.AreEqual("test", evt.GetProperty("Trigger").Value);
        }

        [TestMethod]
        public void EventsInheritedFromPrototype()
        {
            Entity parent = new Entity() { Name = "parent" };

            Entity child = new Entity();
            child.AddPrototype(parent);

            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parent.AddEvent(parentEvent);

            Assert.AreEqual(1, parent.Events.Count(x => x.IsLocal));
            Assert.AreEqual(1, child.Events.Count(x => x.IsInherited));
        }

        [TestMethod]
        public void EventsInheritedFromAllPrototypes()
        {
            Entity parent = new Entity() { Name = "parent" };
            Entity otherParent = new Entity() { Name = "otherParent" };

            Entity child = new Entity();
            child.AddPrototype(parent);
            child.AddPrototype(otherParent);

            parent.AddEvent(new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType)));
            otherParent.AddEvent(new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType)));

            Assert.AreEqual(1, parent.Events.Count(x => x.IsLocal));
            Assert.AreEqual(1, otherParent.Events.Count(x => x.IsLocal));
            Assert.AreEqual(2, child.Events.Count(x => x.IsInherited));
        }

        [TestMethod]
        public void RemoveInheritedEvent()
        {
            Entity parent = new Entity() { Name = "parent" };

            Entity child = new Entity();
            child.AddPrototype(parent);

            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parent.AddEvent(parentEvent);
            parent.RemoveEvent(parentEvent);

            Assert.AreEqual(0, parent.Events.Count);
            Assert.AreEqual(0, child.Events.Count);
        }

        [TestMethod]
        public void EventRemovedAfterRemovePrototype()
        {
            Entity parent = new Entity() { Name = "parent" };

            Entity child = new Entity();
            child.AddPrototype(parent);

            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parent.AddEvent(parentEvent);

            child.RemovePrototype(parent);

            Assert.AreEqual(1, parent.Events.Count);
            Assert.AreEqual(0, child.Events.Count);
        }

        [TestMethod]
        public void EventsAddedAfterAddPrototype()
        {
            Entity parent = new Entity() { Name = "parent" };

            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parent.AddEvent(parentEvent);

            Entity child = new Entity();
            child.AddPrototype(parent);

            Assert.AreEqual(1, parent.Events.Count(x => x.IsLocal));
            Assert.AreEqual(1, child.Events.Count(x => x.IsInherited));
        }

        [TestMethod]
        public void CannotSetInheritedEventProperty()
        {
            Entity parent = new Entity() { Name = "parent" };

            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parentEvent.SetProperty("Trigger", "test");
            parent.AddEvent(parentEvent);

            Entity child = new Entity();
            child.AddPrototype(parent);

            AbstractProperty childProperty = child.Events.Single().GetProperty("Trigger");
            childProperty.Value = "test2";

            Assert.AreEqual("test", childProperty.Value);
        }

        [TestMethod]
        public void CannotRemoveInheritedEventFromInheritingEntity()
        {
            Entity parent = new Entity() { Name = "parent" };

            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parentEvent.SetProperty("Trigger", "test");
            parent.AddEvent(parentEvent);

            Entity child = new Entity();
            child.AddPrototype(parent);

            AbstractEvent childEvent = child.Events.Single();
            child.RemoveEvent(childEvent);

            Assert.AreEqual(1, child.Events.Count);
        }
    }
}
