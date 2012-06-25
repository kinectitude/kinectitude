using System.Linq;
using EditorModels.ViewModels;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EditorModels.Tests
{
    [TestClass]
    public class PropertyViewModelTests
    {
        [Plugin("Dependent Manager", "")]
        private class DependentManager : Manager<DependentComponent> { }

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
            property.IsInherited = false;
            property.Value = 500;

            Assert.AreEqual(500, component.Component.Properties.Single(x => x.Name == "X").Value);
        }

        [TestMethod]
        public void CannotSetValueForInheritedProperty()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            ComponentViewModel parentComponent = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            
            PropertyViewModel parentProperty = parentComponent.GetProperty("X");
            parentProperty.IsInherited = false;
            parentProperty.Value = 500;
            parent.AddComponent(parentComponent);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            ComponentViewModel childComponent = child.GetComponentByType(TransformComponentType);
            
            PropertyViewModel childProperty = childComponent.GetProperty("X");
            childProperty.Value = 250;

            Assert.AreEqual(500, childProperty.Value);
            Assert.AreEqual(0, childComponent.Component.Properties.Count());
        }

        [TestMethod]
        public void ValueFollowsInheritedProperty()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            ComponentViewModel parentComponent = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            
            PropertyViewModel parentProperty = parentComponent.GetProperty("X");
            parentProperty.IsInherited = false;
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
            parentProperty.IsInherited = false;
            parentProperty.Value = 500;
            parent.AddComponent(parentComponent);

            EntityViewModel otherParent = new EntityViewModel() { Name = "otherParent" };
            
            ComponentViewModel otherParentComponent = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            
            PropertyViewModel otherParentProperty = otherParentComponent.GetProperty("X");
            otherParentProperty.IsInherited = false;
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
            Assert.AreEqual(0, component.Component.Properties.Count());

            property.IsInherited = false;

            Assert.IsFalse(property.IsInherited);
            Assert.AreEqual(1, component.Component.Properties.Count());
        }

        [TestMethod]
        public void SetLocalPropertyToInherited()
        {
            ComponentViewModel component = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            
            PropertyViewModel property = component.GetProperty("X");
            property.IsInherited = false;

            Assert.IsFalse(property.IsInherited);
            Assert.AreEqual(1, component.Component.Properties.Count());

            property.IsInherited = true;

            Assert.IsTrue(property.IsInherited);
            Assert.AreEqual(0, component.Component.Properties.Count());
        }
    }
}
