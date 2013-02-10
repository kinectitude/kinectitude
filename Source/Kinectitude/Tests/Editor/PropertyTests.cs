﻿using System.Linq;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Components;
using Kinectitude.Editor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Component = Kinectitude.Editor.Models.Component;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Values;

namespace Kinectitude.Editor.Tests
{
    /*
    [TestClass]
    public class PropertyTests
    {
        [Plugin("Dependent Manager", "")]
        private class DependentManager : Kinectitude.Core.Base.Manager<DependentComponent>
        {
            public override void OnUpdate(float frameDelta) { }
        }

        [Plugin("Dependent Component", "")]
        [Requires(typeof(TransformComponent))]
        [Requires(typeof(DependentManager))]
        private class DependentComponent : Kinectitude.Core.Base.Component
        {
            public override void Destroy() { }
        }

        private static readonly string TransformComponentType = typeof(TransformComponent).FullName;
        private static readonly string DependentComponentType = typeof(DependentComponent).FullName;
        private static readonly string DependentManagerType = typeof(DependentManager).FullName;

        [TestMethod]
        public void SetValue()
        {
            bool eventFired = false;

            Component component = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            
            Property property = component.GetProperty("X");
            property.PropertyChanged += (o, e) => eventFired |= (e.PropertyName == "Value");

            property.Value = new Value("500");

            Assert.IsTrue(eventFired);
            Assert.AreEqual(500, component.Properties.Single(x => x.Name == "X").Value.Reader.GetIntValue());
            //Assert.IsTrue(property.IsRoot);
            Assert.IsFalse(property.IsReadOnly);
        }

        [TestMethod]
        public void DefaultPropertyOnRootComponent()
        {
            Component component = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            Property property = component.GetProperty("X");

            Assert.IsFalse(property.HasOwnValue);
        }

        [TestMethod]
        public void DefaultPropertyBecomesInheritedOnPrototypeChange()
        {
            Entity child = new Entity();
            
            Plugin plugin = Workspace.Instance.GetPlugin(TransformComponentType);

            Component childComponent = new Component(plugin);
            child.AddComponent(childComponent);

            Property childProperty = childComponent.GetProperty("X");

            Assert.IsFalse(childProperty.HasOwnValue);
            Assert.AreEqual(childProperty.PluginProperty.DefaultValue.Reader.GetPreferedValue(), childProperty.Value.Reader.GetPreferedValue());

            Entity parent = new Entity() { Name = "parent" };

            Component parentComponent = new Component(plugin);

            Property parentProperty = parentComponent.GetProperty("X");
            parentProperty.Value = new Value("500");

            parent.AddComponent(parentComponent);

            child.AddPrototype(parent);

            Assert.IsFalse(childProperty.HasOwnValue);
            Assert.AreEqual(500, childProperty.Value.Reader.GetIntValue());
        }

        //[TestMethod]
        //public void CannotSetValueForInheritedProperty()
        //{
        //    Entity parent = new Entity() { Name = "parent" };
            
        //    Component parentComponent = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            
        //    Property parentProperty = parentComponent.GetProperty("X");
        //    parentProperty.Value = 500;
        //    parent.AddComponent(parentComponent);

        //    Entity child = new Entity();
        //    child.AddPrototype(parent);

        //    Component childComponent = child.GetComponentByType(TransformComponentType);
            
        //    Property childProperty = childComponent.GetProperty("X");
        //    childProperty.Value = 250;

        //    Assert.IsTrue(childProperty.IsInherited);
        //    //Assert.IsTrue(childProperty.CanInherit);
        //    Assert.AreEqual(500, childProperty.Value);
        //    Assert.AreEqual(0, childComponent.Properties.Count(x => x.IsLocal));
        //}

        [TestMethod]
        public void ValueFollowsInheritedProperty()
        {
            Entity parent = new Entity() { Name = "parent" };
            
            Component parentComponent = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            
            Property parentProperty = parentComponent.GetProperty("X");
            parentProperty.Value = new Value("500");
            parent.AddComponent(parentComponent);

            Entity child = new Entity();
            child.AddPrototype(parent);
            
            Component childComponent = child.GetComponentByType(TransformComponentType);
            
            Property childProperty = childComponent.GetProperty("X");

            Assert.AreEqual(500, childProperty.Value.Reader.GetIntValue());

            parentProperty.Value = new Value("250");

            Assert.AreEqual(250, childProperty.Value.Reader.GetIntValue());
        }

        [TestMethod]
        public void ValueFollowsInheritedComponentChange()
        {
            Entity parent = new Entity() { Name = "parent" };
            
            Component parentComponent = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            
            Property parentProperty = parentComponent.GetProperty("X");
            parentProperty.Value = new Value("500");
            parent.AddComponent(parentComponent);

            Entity otherParent = new Entity() { Name = "otherParent" };
            
            Component otherParentComponent = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            
            Property otherParentProperty = otherParentComponent.GetProperty("X");
            otherParentProperty.Value = new Value("250");
            otherParent.AddComponent(otherParentComponent);

            Entity child = new Entity();
            child.AddPrototype(parent);

            Component childComponent = child.GetComponentByType(TransformComponentType);
            
            Property childProperty = childComponent.GetProperty("X");

            Assert.AreEqual(500, childProperty.Value.Reader.GetIntValue());

            child.RemovePrototype(parent);
            child.AddPrototype(otherParent);
            childComponent = child.GetComponentByType(TransformComponentType);
            childProperty = childComponent.GetProperty("X");

            Assert.AreEqual(250, childProperty.Value.Reader.GetIntValue());
        }

        [TestMethod]
        public void GiveDefaultPropertyLocalValue()
        {
            Component component = new Component(Workspace.Instance.GetPlugin(TransformComponentType));

            Property property = component.GetProperty("X");

            Assert.IsFalse(property.HasOwnValue);

            property.Value = new Value("5");

            Assert.IsTrue(property.HasOwnValue);
        }

        [TestMethod]
        public void ClearLocalValueFromProperty()
        {
            Entity parent = new Entity() { Name = "parent" };

            Component parentComponent = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            parentComponent.SetProperty("X", new Value("500"));

            parent.AddComponent(parentComponent);

            Entity child = new Entity();

            Component childComponent = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            Property childProperty = childComponent.GetProperty("X");
            childProperty.Value = new Value("250");

            child.AddComponent(childComponent);

            Assert.IsTrue(childProperty.HasOwnValue);
            Assert.AreEqual(250, childProperty.Value.Reader.GetIntValue());

            child.AddPrototype(parent);

            Assert.IsTrue(childProperty.HasOwnValue);
            Assert.AreEqual(250, childProperty.Value.Reader.GetIntValue());

            childProperty.Value = null;

            Assert.IsFalse(childProperty.HasOwnValue);
            Assert.AreEqual(500, childProperty.Value.Reader.GetIntValue());
        }

        [TestMethod]
        public void CreateDefaultProperty()
        {
            
        }

        [TestMethod]
        public void CreateBooleanProperty()
        {
            
        }

        [TestMethod]
        public void CreateEnumerationProperty()
        {
            
        }

        [TestMethod]
        public void CreateKeyProperty()
        {
            
        }

        [TestMethod]
        public void CreateExpressionProperty()
        {
            
        }

        [TestMethod]
        public void CreateEntitySelectorProperty()
        {
            
        }
    }*/
}
