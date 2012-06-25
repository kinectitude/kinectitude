using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models.Base;
using Kinectitude.Editor.ViewModels;
using Kinectitude.Core.Components;
using Kinectitude.Editor.Models.Plugins;

namespace Kinectitude.Tests.Editor
{
    [TestClass]
    public class ComponentViewModelTests
    {
        [TestMethod]
        public void ComponentHasDefaultInheritedProperties()
        {
            PluginDescriptor descriptor = new PluginDescriptor(typeof(TransformComponent));
            Entity entity = new Entity();

            ComponentViewModel componentViewModel = new ComponentViewModel(entity, descriptor);

            Assert.AreNotEqual(componentViewModel.Properties.Count(), 0);

            foreach (ComponentPropertyViewModel propertyViewModel in componentViewModel.Properties)
            {
                if (propertyViewModel.IsLocal)
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void AddNewComponentToEntity()
        {
            PluginDescriptor descriptor = new PluginDescriptor(typeof(TransformComponent));

            Entity entity = new Entity();
            ComponentViewModel componentViewModel = new ComponentViewModel(entity, descriptor);

            Assert.IsNull(entity.GetComponent(descriptor));

            componentViewModel.AddComponent();

            Component component = entity.GetComponent(descriptor);

            Assert.IsNotNull(component);
            Assert.IsFalse(componentViewModel.IsInherited);
            Assert.IsTrue(componentViewModel.IsLocal);
        }

        [TestMethod]
        public void RemoveComponentFromEntity()
        {
            PluginDescriptor descriptor = new PluginDescriptor(typeof(TransformComponent));

            Entity entity = new Entity();
            entity.AddComponent(new Component(descriptor));

            ComponentViewModel componentViewModel = EntityViewModel.GetViewModel(entity).GetComponentViewModel(descriptor);
            componentViewModel.RemoveComponent();

            Component component = entity.GetComponent(descriptor);

            Assert.IsNull(component);
        }

        [TestMethod]
        public void ExistingInheritedComponent()
        {
            PluginDescriptor descriptor = new PluginDescriptor(typeof(TransformComponent));

            Entity parent = new Entity() { Name = "parent" };
            parent.AddComponent(new Component(descriptor));

            Entity child = new Entity();
            child.AddPrototype(parent);

            ComponentViewModel componentViewModel = EntityViewModel.GetViewModel(child).GetComponentViewModel(descriptor);

            Assert.IsNotNull(componentViewModel);
            Assert.IsTrue(componentViewModel.IsInherited);
        }

        [TestMethod]
        public void ExistingLocalComponent()
        {
            PluginDescriptor descriptor = new PluginDescriptor(typeof(TransformComponent));

            Entity entity = new Entity();
            entity.AddComponent(new Component(descriptor));

            ComponentViewModel componentViewModel = EntityViewModel.GetViewModel(entity).GetComponentViewModel(descriptor);

            Assert.IsNotNull(componentViewModel);
            Assert.IsTrue(componentViewModel.IsLocal);
            Assert.IsFalse(componentViewModel.IsInherited);
        }

        [TestMethod]
        public void AddedComponentVisibleInChildEntity()
        {
            PluginDescriptor descriptor = new PluginDescriptor(typeof(TransformComponent));

            Entity parent = new Entity() { Name = "parent" };

            Entity child = new Entity();
            child.AddPrototype(parent);

            EntityViewModel childEntityViewModel = EntityViewModel.GetViewModel(child);

            Assert.AreEqual(childEntityViewModel.Components.Count, 0);

            EntityViewModel parentEntityViewModel = EntityViewModel.GetViewModel(parent);
            ComponentViewModel componentViewModel = new ComponentViewModel(parent, descriptor);
            parentEntityViewModel.AddComponent(componentViewModel);

            Assert.AreEqual(childEntityViewModel.Components.Count, 1);
        }

        [TestMethod]
        public void RemovedComponentRemovedFromChildEntity()
        {
            PluginDescriptor descriptor = new PluginDescriptor(typeof(TransformComponent));

            Entity parent = new Entity() { Name = "parent" };
            parent.AddComponent(new Component(descriptor));

            Entity child = new Entity();
            child.AddPrototype(parent);

            EntityViewModel childEntityViewModel = EntityViewModel.GetViewModel(child);

            Assert.AreEqual(childEntityViewModel.Components.Count, 1);

            EntityViewModel parentEntityViewModel = EntityViewModel.GetViewModel(parent);
            ComponentViewModel componentViewModel = parentEntityViewModel.GetComponentViewModel(descriptor);
            parentEntityViewModel.RemoveComponent(componentViewModel);

            Assert.AreEqual(childEntityViewModel.Components.Count, 0);
        }

        [TestMethod]
        public void ComponentInheritedFromLeftmostPrototype()
        {
            PluginDescriptor descriptor = new PluginDescriptor(typeof(TransformComponent));

            Entity leftParent = new Entity() { Name = "leftParent" };
            Component leftComponent = new Component(descriptor);
            leftParent.AddComponent(leftComponent);

            Entity rightParent = new Entity() { Name = "rightParent" };
            Component rightComponent = new Component(descriptor);
            rightParent.AddComponent(rightComponent);

            Entity child = new Entity();
            child.AddPrototype(leftParent);
            child.AddPrototype(rightParent);

            EntityViewModel childEntityViewModel = EntityViewModel.GetViewModel(child);

            Assert.AreEqual(childEntityViewModel.Components.Count, 1);

            ComponentViewModel componentViewModel = EntityViewModel.GetViewModel(child).GetComponentViewModel(descriptor);
            
            Assert.AreEqual(componentViewModel.Component, leftComponent);
        }
    }
}
