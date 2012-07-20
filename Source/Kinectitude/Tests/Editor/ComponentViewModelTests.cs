using System.Linq;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using Kinectitude.Editor.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kinectitude.Editor.Tests
{
    [TestClass]
    public class ComponentViewModelTests
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
        private static readonly string TransformComponentShort = typeof(TransformComponent).Name;
        private static readonly string DependentComponentType = typeof(DependentComponent).FullName;
        private static readonly string DependentManagerType = typeof(DependentManager).FullName;

        public ComponentViewModelTests()
        {
            Workspace.Instance.AddPlugin(new PluginViewModel(typeof(DependentComponent)));
            Workspace.Instance.AddPlugin(new PluginViewModel(typeof(DependentManager)));
        }

        [TestMethod]
        public void AddLocalComponent()
        {
            EntityViewModel entity = new EntityViewModel();
            entity.AddComponent(new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType)));

            Assert.AreEqual(1, entity.Components.Count(x => x.Type == TransformComponentType));
        }

        [TestMethod]
        public void RemoveLocalComponent()
        {
            EntityViewModel entity = new EntityViewModel();
            
            ComponentViewModel component = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            entity.AddComponent(component);
            entity.RemoveComponent(component);

            Assert.AreEqual(0, entity.Components.Count);
        }

        [TestMethod]
        public void RemoveRootComponent()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            ComponentViewModel component = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            parent.AddComponent(component);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            parent.RemoveComponent(component);

            Assert.AreEqual(0, parent.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(0, child.Components.Count(x => x.Type == TransformComponentType));
        }

        [TestMethod]
        public void RemoveRootComponentWhenInheritedComponentHasProperties()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            ComponentViewModel component = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            parent.AddComponent(component);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            ComponentViewModel childComponent = child.GetComponentByType(TransformComponentType);
            PropertyViewModel childProperty = childComponent.GetProperty("X");
            childProperty.IsInherited = false;
            childProperty.Value = 500;

            parent.RemoveComponent(component);

            Assert.AreEqual(0, parent.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(1, child.Components.Count(x => x.IsRoot));
            Assert.AreEqual(1, child.Components.Count(x => x.Type == TransformComponentType));
        }

        [TestMethod]
        public void CannotRemoveInheritedComponent()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            parent.AddComponent(new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType)));

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            ComponentViewModel inheritedComponent = child.GetComponentByType(TransformComponentType);
            child.RemoveComponent(inheritedComponent);

            Assert.AreEqual(1, parent.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(1, child.Components.Count(x => x.Type == TransformComponentType));
        }

        [TestMethod]
        public void CannotAddMultipleComponentsPerRole()
        {
            EntityViewModel entity = new EntityViewModel();
            
            ComponentViewModel component = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            entity.AddComponent(component);
            
            entity.AddComponent(new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType)));

            Assert.AreEqual(component, entity.Components.Single());
        }

        [TestMethod]
        public void CannotRemoveComponentWithDependency()
        {
            EntityViewModel entity = new EntityViewModel();
            entity.AddComponent(new ComponentViewModel(Workspace.Instance.GetPlugin(DependentComponentType)));

            ComponentViewModel transform = entity.GetComponentByType(TransformComponentType);
            entity.RemoveComponent(transform);

            Assert.AreEqual(1, entity.Components.Count(x => x.Type == TransformComponentType));
        }

        [TestMethod]
        public void EntityInheritsComponentFromPrototype()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            parent.AddComponent(new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType)));

            Assert.AreEqual(1, parent.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(1, child.Components.Count(x => x.Type == TransformComponentType));
        }

        [TestMethod]
        public void EntityInheritsExistingComponentFromPrototype()
        {
            /*EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            parent.AddComponent(new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType)));

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            Assert.AreEqual(1, parent.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(1, parent.Entity.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(1, child.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(0, child.Entity.Components.Count(x => x.Type == TransformComponentType));*/

            GameViewModel game = new GameViewModel("Test Game")
            {
                Width = 800,
                Height = 600,
                IsFullScreen = false
            };

            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            ComponentViewModel parentComponent = new ComponentViewModel(game.GetPlugin(typeof(TransformComponent).FullName));
            
            PropertyViewModel x = parentComponent.GetProperty("X");
            x.IsInherited = false;
            x.Value = 400;
            parent.AddComponent(parentComponent);
            
            game.AddPrototype(parent);

            SceneViewModel scene = new SceneViewModel("Test Scene");

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);
            
            ComponentViewModel childComponent = child.Components[0];
            
            PropertyViewModel y = childComponent.GetProperty("Y");
            y.IsInherited = false;
            y.Value = 300;
            
            scene.AddEntity(child);

            game.AddScene(scene);
            game.FirstScene = scene;
        }

        [TestMethod]
        public void ComponentFollowsDefineChange()
        {
            GameViewModel game = new GameViewModel("Test Game");

            UsingViewModel use = new UsingViewModel() { File = "Kinectitude.Core.dll" };

            DefineViewModel define = new DefineViewModel(TransformComponentShort, TransformComponentType);
            use.AddDefine(define);
            
            game.AddUsing(use);

            EntityViewModel entity = new EntityViewModel() { Name = "parent" };
            game.AddPrototype(entity);

            ComponentViewModel component = new ComponentViewModel(game.GetPlugin(TransformComponentShort));
            entity.AddComponent(component);

            Assert.AreEqual(TransformComponentShort, component.Type);

            define.Name = "tc";

            Assert.AreEqual("tc", component.Type);
        }

        [TestMethod]
        public void ComponentBecomesInheritedAfterAddToParent()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };

            EntityViewModel child = new EntityViewModel();
            ComponentViewModel childComponent = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            child.AddComponent(childComponent);
            child.AddPrototype(parent);

            Assert.IsFalse(childComponent.IsInherited);

            parent.AddComponent(new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType)));

            Assert.IsTrue(childComponent.IsInherited);
        }

        [TestMethod]
        public void ComponentBecomesInheritableAfterPrototypeChange()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            parent.AddComponent(new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType)));

            EntityViewModel child = new EntityViewModel();
            ComponentViewModel component = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            child.AddComponent(component);

            Assert.IsFalse(component.IsInherited);

            child.AddPrototype(parent);

            Assert.IsTrue(component.IsInherited);
        }

        [TestMethod]
        public void ComponentBecomesNonInheritableAfterRemoveFromParent()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            ComponentViewModel parentComponent = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            parent.AddComponent(parentComponent);

            EntityViewModel child = new EntityViewModel();
            
            ComponentViewModel childComponent = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            
            PropertyViewModel property = childComponent.GetProperty("X");
            property.Value = 500;
            
            child.AddComponent(childComponent);
            child.AddPrototype(parent);

            Assert.IsTrue(childComponent.IsInherited);

            parent.RemoveComponent(parentComponent);

            Assert.IsFalse(childComponent.IsInherited);
        }

        [TestMethod]
        public void ComponentBecomesNonInheritableAfterPrototypeChange()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            parent.AddComponent(new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType)));

            EntityViewModel child = new EntityViewModel();
            
            ComponentViewModel component = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            
            PropertyViewModel property = component.GetProperty("X");
            property.Value = 500;
            
            child.AddComponent(component);
            child.AddPrototype(parent);

            Assert.IsTrue(component.IsInherited);

            child.RemovePrototype(parent);

            Assert.IsFalse(component.IsInherited);
        }

        [TestMethod]
        public void AddingComponentAddsRequiredComponent()
        {
            EntityViewModel entity = new EntityViewModel();
            entity.AddComponent(new ComponentViewModel(Workspace.Instance.GetPlugin(DependentComponentType)));

            Assert.AreEqual(1, entity.Components.Count(x => x.Type == DependentComponentType));
            Assert.AreEqual(1, entity.Components.Count(x => x.Type == TransformComponentType));
        }

        [TestMethod]
        public void AddingComponentAddsRequiredManagers()
        {
            SceneViewModel scene = new SceneViewModel("Test Scene");
            
            EntityViewModel entity = new EntityViewModel();
            entity.AddComponent(new ComponentViewModel(Workspace.Instance.GetPlugin(DependentComponentType)));

            scene.AddEntity(entity);

            Assert.AreEqual(1, scene.Managers.Count(x => x.Type == DependentManagerType));
        }
    }
}
