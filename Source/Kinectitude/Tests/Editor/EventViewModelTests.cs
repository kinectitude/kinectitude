using System.Linq;
using Kinectitude.Editor.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kinectitude.Editor.Tests
{
    [TestClass]
    public class EventViewModelTests
    {
        private static readonly string TriggerOccursEventType = "Kinectitude.Core.Events.TriggerOccursEvent";

        [TestMethod]
        public void AddLocalEvent()
        {
            EntityViewModel entity = new EntityViewModel();

            EventViewModel evt = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            entity.AddEvent(evt);

            Assert.IsTrue(evt.IsLocal);
            Assert.AreEqual(1, entity.Events.Count);
        }

        [TestMethod]
        public void RemoveLocalEvent()
        {
            EntityViewModel entity = new EntityViewModel();

            EventViewModel evt = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            entity.AddEvent(evt);
            entity.RemoveEvent(evt);

            Assert.AreEqual(0, entity.Events.Count);
        }

        [TestMethod]
        public void AddMultipleEventsOfSameType()
        {
            EntityViewModel entity = new EntityViewModel();

            entity.AddEvent(new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType)));
            entity.AddEvent(new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType)));

            Assert.AreEqual(2, entity.Events.Count);
        }

        [TestMethod]
        public void SetEventProperty()
        {
            EventViewModel evt = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            evt.SetProperty("Trigger", "test");

            Assert.AreEqual("test", evt.GetProperty("Trigger").Value);
        }

        [TestMethod]
        public void EventsInheritedFromPrototype()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            EventViewModel parentEvent = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parent.AddEvent(parentEvent);

            Assert.AreEqual(1, parent.Events.Count(x => x.IsLocal));
            Assert.AreEqual(1, child.Events.Count(x => x.IsInherited));
        }

        [TestMethod]
        public void EventsInheritedFromAllPrototypes()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            EntityViewModel otherParent = new EntityViewModel() { Name = "otherParent" };

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);
            child.AddPrototype(otherParent);

            parent.AddEvent(new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType)));
            otherParent.AddEvent(new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType)));

            Assert.AreEqual(1, parent.Events.Count(x => x.IsLocal));
            Assert.AreEqual(1, otherParent.Events.Count(x => x.IsLocal));
            Assert.AreEqual(2, child.Events.Count(x => x.IsInherited));
        }

        [TestMethod]
        public void RemoveInheritedEvent()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            EventViewModel parentEvent = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parent.AddEvent(parentEvent);
            parent.RemoveEvent(parentEvent);

            Assert.AreEqual(0, parent.Events.Count);
            Assert.AreEqual(0, child.Events.Count);
        }

        [TestMethod]
        public void EventRemovedAfterRemovePrototype()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            EventViewModel parentEvent = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parent.AddEvent(parentEvent);

            child.RemovePrototype(parent);

            Assert.AreEqual(1, parent.Events.Count);
            Assert.AreEqual(0, child.Events.Count);
        }

        [TestMethod]
        public void EventsAddedAfterAddPrototype()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };

            EventViewModel parentEvent = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parent.AddEvent(parentEvent);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            Assert.AreEqual(1, parent.Events.Count(x => x.IsLocal));
            Assert.AreEqual(1, child.Events.Count(x => x.IsInherited));
        }

        [TestMethod]
        public void CannotSetInheritedEventProperty()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };

            EventViewModel parentEvent = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parentEvent.SetProperty("Trigger", "test");
            parent.AddEvent(parentEvent);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            AbstractPropertyViewModel childProperty = child.Events.Single().GetProperty("Trigger");
            childProperty.Value = "test2";

            Assert.AreEqual("test", childProperty.Value);
        }

        [TestMethod]
        public void CannotRemoveInheritedEventFromInheritingEntity()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };

            EventViewModel parentEvent = new EventViewModel(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            parentEvent.SetProperty("Trigger", "test");
            parent.AddEvent(parentEvent);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            AbstractEventViewModel childEvent = child.Events.Single();
            child.RemoveEvent(childEvent);

            Assert.AreEqual(1, child.Events.Count);
        }
    }
}
