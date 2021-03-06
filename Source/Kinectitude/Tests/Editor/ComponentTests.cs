//-----------------------------------------------------------------------
// <copyright file="ComponentTests.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Components;
using Kinectitude.Editor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Values;

namespace Kinectitude.Editor.Tests
{
    [TestClass]
    public class ComponentTests
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
        private static readonly string TransformComponentShort = typeof(TransformComponent).Name;
        private static readonly string DependentComponentType = typeof(DependentComponent).FullName;
        private static readonly string DependentManagerType = typeof(DependentManager).FullName;

        public ComponentTests()
        {
            Workspace.Instance.AddPlugin(new Plugin(typeof(DependentComponent)));
            Workspace.Instance.AddPlugin(new Plugin(typeof(DependentManager)));
        }

        [TestMethod]
        public void AddLocalComponent()
        {
            Entity entity = new Entity();
            entity.AddComponent(new Component(Workspace.Instance.GetPlugin(TransformComponentType)));

            Assert.AreEqual(1, entity.Components.Count(x => x.Type == TransformComponentType));
        }

        [TestMethod]
        public void RemoveLocalComponent()
        {
            Entity entity = new Entity();
            
            Component component = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            entity.AddComponent(component);
            entity.RemoveComponent(component);

            Assert.AreEqual(0, entity.Components.Count);
        }

        [TestMethod]
        public void RemoveRootComponent()
        {
            Entity parent = new Entity() { Name = "parent" };
            
            Component component = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            parent.AddComponent(component);

            Entity child = new Entity();
            child.AddPrototype(parent);

            parent.RemoveComponent(component);

            Assert.AreEqual(0, parent.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(0, child.Components.Count(x => x.Type == TransformComponentType));
        }

        [TestMethod]
        public void RemoveRootComponentWhenInheritedComponentHasProperties()
        {
            Entity parent = new Entity() { Name = "parent" };
            
            Component component = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            parent.AddComponent(component);

            Entity child = new Entity();
            child.AddPrototype(parent);

            Component childComponent = child.GetComponentByType(TransformComponentType);
            Property childProperty = childComponent.GetProperty("X");
            //childProperty.IsInherited = false;
            childProperty.Value = new Value("500");

            parent.RemoveComponent(component);

            Assert.AreEqual(0, parent.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(1, child.Components.Count(x => x.IsRoot));
            Assert.AreEqual(1, child.Components.Count(x => x.Type == TransformComponentType));
        }

        [TestMethod]
        public void CannotRemoveInheritedComponent()
        {
            Entity parent = new Entity() { Name = "parent" };
            parent.AddComponent(new Component(Workspace.Instance.GetPlugin(TransformComponentType)));

            Entity child = new Entity();
            child.AddPrototype(parent);

            Component inheritedComponent = child.GetComponentByType(TransformComponentType);
            child.RemoveComponent(inheritedComponent);

            Assert.AreEqual(1, parent.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(1, child.Components.Count(x => x.Type == TransformComponentType));
        }

        [TestMethod]
        public void CannotAddMultipleComponentsPerRole()
        {
            Entity entity = new Entity();
            
            Component component = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            entity.AddComponent(component);
            
            entity.AddComponent(new Component(Workspace.Instance.GetPlugin(TransformComponentType)));

            Assert.AreEqual(component, entity.Components.Single());
        }

        [TestMethod]
        public void CannotRemoveComponentWithDependency()
        {
            Entity entity = new Entity();
            entity.AddComponent(new Component(Workspace.Instance.GetPlugin(DependentComponentType)));

            Component transform = entity.GetComponentByType(TransformComponentType);
            entity.RemoveComponent(transform);

            Assert.AreEqual(1, entity.Components.Count(x => x.Type == TransformComponentType));
        }

        [TestMethod]
        public void EntityInheritsComponentFromPrototype()
        {
            Entity parent = new Entity() { Name = "parent" };
            
            Entity child = new Entity();
            child.AddPrototype(parent);

            parent.AddComponent(new Component(Workspace.Instance.GetPlugin(TransformComponentType)));

            Assert.AreEqual(1, parent.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(1, child.Components.Count(x => x.Type == TransformComponentType));
        }

        [TestMethod]
        public void EntityInheritsExistingComponentFromPrototype()
        {
            /*Entity parent = new Entity() { Name = "parent" };
            parent.AddComponent(new Component(Workspace.Instance.GetPlugin(TransformComponentType)));

            Entity child = new Entity();
            child.AddPrototype(parent);

            Assert.AreEqual(1, parent.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(1, parent.Entity.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(1, child.Components.Count(x => x.Type == TransformComponentType));
            Assert.AreEqual(0, child.Entity.Components.Count(x => x.Type == TransformComponentType));*/

            Game game = new Game("Test Game");

            Entity parent = new Entity() { Name = "parent" };
            
            Component parentComponent = new Component(game.GetPlugin(typeof(TransformComponent).FullName));
            
            Property x = parentComponent.GetProperty("X");
            x.Value = new Value("400");

            parent.AddComponent(parentComponent);
            
            game.AddPrototype(parent);

            Scene scene = new Scene("Test Scene");

            Entity child = new Entity();
            child.AddPrototype(parent);
            
            Component childComponent = child.Components[0];
            
            Property y = childComponent.GetProperty("Y");
            y.Value = new Value("300");
            
            scene.AddEntity(child);

            game.AddScene(scene);
            game.FirstScene = scene;
        }

        [TestMethod]
        public void ComponentFollowsDefineChange()
        {
            Game game = new Game("Test Game");

            Using use = new Using() { File = "Kinectitude.Core.dll" };

            Define define = new Define(TransformComponentShort, TransformComponentType);
            use.AddDefine(define);
            
            game.AddUsing(use);

            Entity entity = new Entity() { Name = "parent" };
            game.AddPrototype(entity);

            Component component = new Component(game.GetPlugin(TransformComponentShort));
            entity.AddComponent(component);

            Assert.AreEqual(TransformComponentShort, component.Type);

            define.Name = "tc";

            Assert.AreEqual("tc", component.Type);
        }

        [TestMethod]
        public void ComponentBecomesInheritedAfterAddToParent()
        {
            Entity parent = new Entity() { Name = "parent" };

            Entity child = new Entity();
            Component childComponent = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            child.AddComponent(childComponent);
            child.AddPrototype(parent);

            Assert.IsFalse(childComponent.IsInherited);

            parent.AddComponent(new Component(Workspace.Instance.GetPlugin(TransformComponentType)));

            Assert.IsTrue(childComponent.IsInherited);
        }

        [TestMethod]
        public void ComponentBecomesInheritableAfterPrototypeChange()
        {
            Entity parent = new Entity() { Name = "parent" };
            parent.AddComponent(new Component(Workspace.Instance.GetPlugin(TransformComponentType)));

            Entity child = new Entity();
            Component component = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            child.AddComponent(component);

            Assert.IsFalse(component.IsInherited);

            child.AddPrototype(parent);

            Assert.IsTrue(component.IsInherited);
        }

        [TestMethod]
        public void ComponentBecomesNonInheritableAfterRemoveFromParent()
        {
            Entity parent = new Entity() { Name = "parent" };
            
            Component parentComponent = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            parent.AddComponent(parentComponent);

            Entity child = new Entity();
            
            Component childComponent = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            
            Property property = childComponent.GetProperty("X");
            property.Value = new Value("500");
            
            child.AddComponent(childComponent);
            child.AddPrototype(parent);

            Assert.IsTrue(childComponent.IsInherited);

            parent.RemoveComponent(parentComponent);

            Assert.IsFalse(childComponent.IsInherited);
        }

        [TestMethod]
        public void ComponentBecomesNonInheritableAfterPrototypeChange()
        {
            Entity parent = new Entity() { Name = "parent" };
            parent.AddComponent(new Component(Workspace.Instance.GetPlugin(TransformComponentType)));

            Entity child = new Entity();
            
            Component component = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            
            Property property = component.GetProperty("X");
            property.Value = new Value("500");
            
            child.AddComponent(component);
            child.AddPrototype(parent);

            Assert.IsTrue(component.IsInherited);

            child.RemovePrototype(parent);

            Assert.IsFalse(component.IsInherited);
        }

        [TestMethod]
        public void AddingComponentAddsRequiredComponent()
        {
            Entity entity = new Entity();
            entity.AddComponent(new Component(Workspace.Instance.GetPlugin(DependentComponentType)));

            Assert.AreEqual(1, entity.Components.Count(x => x.Type == DependentComponentType));
            Assert.AreEqual(1, entity.Components.Count(x => x.Type == TransformComponentType));
        }

        [TestMethod]
        public void AddingComponentAddsRequiredManagers()
        {
            Scene scene = new Scene("Test Scene");
            
            Entity entity = new Entity();
            entity.AddComponent(new Component(Workspace.Instance.GetPlugin(DependentComponentType)));

            scene.AddEntity(entity);

            string t1 = scene.Managers.Single().Type;
            string t2 = DependentManagerType;
            bool t3 = t1 == t2;

            Assert.AreEqual(1, scene.Managers.Count(x => x.Type == DependentManagerType));
        }
    }
}
