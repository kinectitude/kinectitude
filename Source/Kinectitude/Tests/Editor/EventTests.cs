using System.Linq;
using Kinectitude.Editor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models.Statements.Events;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Values;

namespace Kinectitude.Editor.Tests
{
    //TODO FIX
    /*
    [TestClass]
    public class EventTests
    {
        private static readonly string TriggerOccursEventType = typeof(Kinectitude.Core.Events.TriggerOccursEvent).FullName;

        [TestMethod]
        public void AddLocalEvent()
        {
            bool collectionChanged = false;

            Entity entity = new Entity();
            entity.Events.CollectionChanged += (o, e) => collectionChanged = true;

            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            entity.AddEvent(evt);

            Assert.IsTrue(collectionChanged);
            Assert.IsTrue(evt.IsLocal);
            Assert.AreEqual(1, entity.Events.Count);
        }

        [TestMethod]
        public void RemoveLocalEvent()
        {
            int eventsFired = 0;

            Entity entity = new Entity();
            entity.Events.CollectionChanged += (o, e) => eventsFired++;

            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            entity.AddEvent(evt);
            entity.RemoveEvent(evt);

            Assert.AreEqual(2, eventsFired);
            Assert.AreEqual(0, entity.Events.Count);
        }

        [TestMethod]
        public void AddMultipleEventsOfSameType()
        {
            int eventsFired = 0;

            Entity entity = new Entity();
            entity.Events.CollectionChanged += (o, e) => eventsFired++;

            entity.AddEvent(new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType)));
            entity.AddEvent(new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType)));

            Assert.AreEqual(2, eventsFired);
            Assert.AreEqual(2, entity.Events.Count);
        }

        [TestMethod]
        public void SetEventProperty()
        {
            bool propertyChanged = false;

            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));

            AbstractProperty property = evt.GetProperty("Trigger");
            property.PropertyChanged += (o, e) => propertyChanged |= (e.PropertyName == "Value");

            evt.SetProperty("Trigger", new Value("test", true));

            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("test", property.Value.Reader.GetStrValue());
        }

        [TestMethod]
        public void EventsInheritedFromPrototype()
        {
            bool collectionChanged = false;

            Entity parent = new Entity() { Name = "parent" };

            Entity child = new Entity();
            child.Events.CollectionChanged += (o, e) => collectionChanged = true;

            child.AddPrototype(parent);

            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parent.AddEvent(parentEvent);

            Assert.IsTrue(collectionChanged);
            Assert.AreEqual(1, parent.Events.Count(x => x.IsLocal));
            Assert.AreEqual(1, child.Events.Count(x => x.IsInherited));
        }

        [TestMethod]
        public void EventsInheritedFromAllPrototypes()
        {
            int eventsFired = 0;

            Entity parent = new Entity() { Name = "parent" };
            Entity otherParent = new Entity() { Name = "otherParent" };

            Entity child = new Entity();
            child.Events.CollectionChanged += (o, e) => eventsFired++;

            child.AddPrototype(parent);
            child.AddPrototype(otherParent);

            parent.AddEvent(new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType)));
            otherParent.AddEvent(new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType)));

            Assert.AreEqual(2, eventsFired);
            Assert.AreEqual(1, parent.Events.Count(x => x.IsLocal));
            Assert.AreEqual(1, otherParent.Events.Count(x => x.IsLocal));
            Assert.AreEqual(2, child.Events.Count(x => x.IsInherited));
        }

        [TestMethod]
        public void RemoveReadOnlyEvent()
        {
            int eventsFired = 0;

            Entity parent = new Entity() { Name = "parent" };

            Entity child = new Entity();
            child.Events.CollectionChanged += (o, e) => eventsFired++;

            child.AddPrototype(parent);

            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parent.AddEvent(parentEvent);
            parent.RemoveEvent(parentEvent);

            Assert.AreEqual(2, eventsFired);
            Assert.AreEqual(0, parent.Events.Count);
            Assert.AreEqual(0, child.Events.Count);
        }

        [TestMethod]
        public void EventRemovedAfterRemovePrototype()
        {
            int eventsFired = 0;

            Entity parent = new Entity() { Name = "parent" };

            Entity child = new Entity();
            child.Events.CollectionChanged += (o, e) => eventsFired++;

            child.AddPrototype(parent);

            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parent.AddEvent(parentEvent);

            child.RemovePrototype(parent);

            Assert.AreEqual(2, eventsFired);
            Assert.AreEqual(1, parent.Events.Count);
            Assert.AreEqual(0, child.Events.Count);
        }

        [TestMethod]
        public void EventsAddedAfterAddPrototype()
        {
            bool collectionChanged = false;

            Entity parent = new Entity() { Name = "parent" };

            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parent.AddEvent(parentEvent);

            Entity child = new Entity();
            child.Events.CollectionChanged += (o, e) => collectionChanged = true;

            child.AddPrototype(parent);

            Assert.IsTrue(collectionChanged);
            Assert.AreEqual(1, parent.Events.Count(x => x.IsLocal));
            Assert.AreEqual(1, child.Events.Count(x => x.IsInherited));
        }

        [TestMethod]
        public void CannotSetReadOnlyEventProperty()
        {
            Entity parent = new Entity() { Name = "parent" };

            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parentEvent.SetProperty("Trigger", new Value("test", true));
            parent.AddEvent(parentEvent);

            Entity child = new Entity();
            child.AddPrototype(parent);

            AbstractProperty childProperty = child.Events.Single().GetProperty("Trigger");
            childProperty.Value = new Value("test2", true);

            Assert.AreEqual("test", childProperty.Value.Reader.GetStrValue());
        }

        [TestMethod]
        public void CannotRemoveReadOnlyEventFromInheritingEntity()
        {
            bool collectionChanged = false;

            Entity parent = new Entity() { Name = "parent" };

            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parentEvent.SetProperty("Trigger", new Value("test"));
            parent.AddEvent(parentEvent);

            Entity child = new Entity();
            child.AddPrototype(parent);

            child.Events.CollectionChanged += (o, e) => collectionChanged = true;

            AbstractEvent childEvent = child.Events.Single();
            child.RemoveEvent(childEvent);

            Assert.IsFalse(collectionChanged);
            Assert.AreEqual(1, child.Events.Count);
        }
    }*/
}
