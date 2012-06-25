using System.Linq;
using EditorModels.ViewModels;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EditorModels.Tests
{
    [TestClass]
    public class ComponentViewModelTests
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
            Assert.AreEqual(1, entity.Entity.Components.Count(x => x.Type == TransformComponentType));
        }

        [TestMethod]
        public void RemoveLocalComponent()
        {
            EntityViewModel entity = new EntityViewModel();
            
            ComponentViewModel component = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            entity.AddComponent(component);
            entity.RemoveComponent(component);

            Assert.AreEqual(0, entity.Components.Count);
            Assert.AreEqual(0, entity.Entity.Components.Count());
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
            Assert.AreEqual(0, parent.Entity.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(0, child.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(0, child.Entity.Components.Count(x => x.Type == TransformComponentType));
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
            Assert.AreEqual(1, parent.Entity.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(1, child.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(0, child.Entity.Components.Count(x => x.Type == TransformComponentType));
        }

        [TestMethod]
        public void CannotAddMultipleComponentsPerRole()
        {
            EntityViewModel entity = new EntityViewModel();
            
            ComponentViewModel component = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            entity.AddComponent(component);
            
            entity.AddComponent(new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType)));

            Assert.AreEqual(component, entity.Components.Single());
            Assert.AreEqual(1, entity.Entity.Components.Count(x => x.Type == TransformComponentType));
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
            Assert.AreEqual(1, parent.Entity.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(1, child.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(0, child.Entity.Components.Count(x => x.Type == TransformComponentType));
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
            game.AddUsing(use);

            DefineViewModel define = new DefineViewModel(TransformComponentShort, TransformComponentType);
            use.AddDefine(define);

            EntityViewModel entity = new EntityViewModel() { Name = "parent" };
            game.AddPrototype(entity);

            ComponentViewModel component = new ComponentViewModel(game.GetPlugin(TransformComponentShort));
            entity.AddComponent(component);

            Assert.AreEqual(TransformComponentShort, component.Type);
            Assert.AreEqual(TransformComponentShort, component.Component.Type);

            define.Name = "tc";

            Assert.AreEqual("tc", component.Type);
            Assert.AreEqual("tc", component.Component.Type);
        }

        [TestMethod]
        public void ComponentIsLocalIfPropertyDefined()
        {
            EntityViewModel entity = new EntityViewModel();
            
            ComponentViewModel component = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            entity.AddComponent(component);

            Assert.IsFalse(component.IsLocal);

            PropertyViewModel property = component.GetProperty("X");
            property.IsInherited = false;
            property.Value = 500;

            Assert.IsTrue(component.IsLocal);
        }

        [TestMethod]
        public void ComponentIsInheritedIfNoPropertiesDefined()
        {
            EntityViewModel entity = new EntityViewModel();
            ComponentViewModel component = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));

            Assert.IsTrue(component.IsInherited);   // TODO: Maybe not?
        }

        [TestMethod]
        public void ComponentBecomesInheritableAfterAddToParent()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };

            EntityViewModel child = new EntityViewModel();
            ComponentViewModel childComponent = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            child.AddComponent(childComponent);
            child.AddPrototype(parent);

            Assert.IsFalse(childComponent.CanInherit);

            parent.AddComponent(new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType)));

            Assert.IsTrue(childComponent.CanInherit);
        }

        [TestMethod]
        public void ComponentBecomesInheritableAfterPrototypeChange()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            parent.AddComponent(new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType)));

            EntityViewModel child = new EntityViewModel();
            ComponentViewModel component = new ComponentViewModel(Workspace.Instance.GetPlugin(TransformComponentType));
            child.AddComponent(component);

            Assert.IsFalse(component.CanInherit);

            child.AddPrototype(parent);

            Assert.IsTrue(component.CanInherit);
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

            Assert.IsTrue(childComponent.CanInherit);

            parent.RemoveComponent(parentComponent);

            Assert.IsFalse(childComponent.CanInherit);
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

            Assert.IsTrue(component.CanInherit);

            child.RemovePrototype(parent);

            Assert.IsFalse(component.CanInherit);
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
