using System.Linq;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using Kinectitude.Editor.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kinectitude.Editor.Tests
{
    [TestClass]
    public class PropertyViewModelTests
    {
        [Plugin("Dependent Manager", "")]
        private class DependentManager : Manager<DependentComponent>
        {
            public override void OnUpdate(float frameDelta) { }
        }

        [Plugin("Dependent Component", "")]
        [Requires(typeof(TransformComponent))]
        [Requires(typeof(DependentManager))]
        private class DependentComponent : Component
        {
            public override void Destroy() { }
        }

        private static readonly string TransformComponentType = typeof(TransformComponent).FullName;
        private static readonly string DependentComponentType = typeof(DependentComponent).FullName;
        private static readonly string DependentManagerType = typeof(DependentManager).FullName;

        [TestMethod]
        public void SetValue()
        {
            ComponentViewModel component = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            
            PropertyViewModel property = component.GetProperty("X");
            property.Value = 500;

            Assert.AreEqual(500, component.Properties.Single(x => x.Name == "X").Value);
            Assert.IsTrue(property.IsRoot);
            Assert.IsFalse(property.IsInherited);
        }

        [TestMethod]
        public void DefaultPropertyOnRootComponent()
        {
            ComponentViewModel component = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));

            PropertyViewModel property = component.GetProperty("X");

            Assert.AreEqual(0, component.Properties.Count(x => x.IsLocal));
            Assert.IsTrue(property.IsRoot);
            Assert.IsTrue(property.IsInherited);
        }

        [TestMethod]
        public void DefaultPropertyBecomesInheritedOnPrototypeChange()
        {
            EntityViewModel child = new EntityViewModel();
            
            ComponentViewModel childComponent = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            child.AddComponent(childComponent);

            PropertyViewModel childProperty = childComponent.GetProperty("X");

            Assert.IsFalse(childProperty.CanInherit);

            EntityViewModel parent = new EntityViewModel() { Name = "parent" };

            ComponentViewModel parentComponent = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));

            PropertyViewModel parentProperty = parentComponent.GetProperty("X");
            parentProperty.Value = 500;

            parent.AddComponent(parentComponent);

            child.AddPrototype(parent);

            Assert.IsTrue(childProperty.CanInherit);
            Assert.AreEqual(500, childProperty.Value);
            Assert.AreEqual(1, childComponent.Properties.Count(x => x.Name == "X" && x.IsInherited));
            Assert.AreEqual(1, parentComponent.Properties.Count(x => x.Name == "X" && x.IsRoot));
        }

        [TestMethod]
        public void CannotSetValueForInheritedProperty()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            ComponentViewModel parentComponent = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            
            PropertyViewModel parentProperty = parentComponent.GetProperty("X");
            parentProperty.Value = 500;
            parent.AddComponent(parentComponent);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            ComponentViewModel childComponent = child.GetComponentByType(TransformComponentType);
            
            PropertyViewModel childProperty = childComponent.GetProperty("X");
            childProperty.Value = 250;

            Assert.IsTrue(childProperty.IsInherited);
            Assert.IsTrue(childProperty.CanInherit);
            Assert.AreEqual(500, childProperty.Value);
            Assert.AreEqual(0, childComponent.Properties.Count(x => x.IsLocal));
        }

        [TestMethod]
        public void ValueFollowsInheritedProperty()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            ComponentViewModel parentComponent = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            
            PropertyViewModel parentProperty = parentComponent.GetProperty("X");
            parentProperty.Value = 500;
            parent.AddComponent(parentComponent);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);
            
            ComponentViewModel childComponent = child.GetComponentByType(TransformComponentType);
            
            PropertyViewModel childProperty = childComponent.GetProperty("X");

            Assert.AreEqual(500, childProperty.Value);

            parentProperty.Value = 250;

            Assert.AreEqual(250, childProperty.Value);
        }

        [TestMethod]
        public void ValueFollowsInheritedComponentChange()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            ComponentViewModel parentComponent = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            
            PropertyViewModel parentProperty = parentComponent.GetProperty("X");
            parentProperty.Value = 500;
            parent.AddComponent(parentComponent);

            EntityViewModel otherParent = new EntityViewModel() { Name = "otherParent" };
            
            ComponentViewModel otherParentComponent = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            
            PropertyViewModel otherParentProperty = otherParentComponent.GetProperty("X");
            otherParentProperty.Value = 250;
            otherParent.AddComponent(otherParentComponent);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            ComponentViewModel childComponent = child.GetComponentByType(TransformComponentType);
            
            PropertyViewModel childProperty = childComponent.GetProperty("X");

            Assert.AreEqual(500, childProperty.Value);

            child.RemovePrototype(parent);
            child.AddPrototype(otherParent);
            childComponent = child.GetComponentByType(TransformComponentType);
            childProperty = childComponent.GetProperty("X");

            Assert.AreEqual(250, childProperty.Value);
        }

        [TestMethod]
        public void SetInheritedPropertyToLocal()
        {
            ComponentViewModel component = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            
            PropertyViewModel property = component.GetProperty("X");

            Assert.IsTrue(property.IsInherited);

            property.IsInherited = false;

            Assert.IsFalse(property.IsInherited);
        }

        [TestMethod]
        public void SetLocalPropertyToInherited()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };

            ComponentViewModel parentComponent = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            parentComponent.SetProperty("X", 500);

            parent.AddComponent(parentComponent);

            EntityViewModel child = new EntityViewModel();

            ComponentViewModel childComponent = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            PropertyViewModel childProperty = childComponent.GetProperty("X");
            childProperty.Value = 250;

            child.AddComponent(childComponent);

            Assert.IsTrue(childProperty.IsLocal);
            Assert.IsFalse(childProperty.CanInherit);
            Assert.AreEqual(250, childProperty.Value);

            child.AddPrototype(parent);

            Assert.IsTrue(childProperty.CanInherit);
            Assert.IsTrue(childProperty.IsLocal);
            Assert.AreEqual(250, childProperty.Value);

            childProperty.IsInherited = true;

            Assert.IsTrue(childProperty.IsInherited);
            Assert.AreEqual(500, childProperty.Value);
        }
    }
}
