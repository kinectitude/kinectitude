//-----------------------------------------------------------------------
// <copyright file="PropertyTests.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System.Linq;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Components;
using Kinectitude.Editor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Component = Kinectitude.Editor.Models.Component;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Values;

namespace Kinectitude.Editor.Tests
{
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

        // Multiple inheritance tests

        private static Game CreateTestGame()
        {
            var game = new Game("Test Game");

            var prototypeA0 = new Entity() { Name = "prototypeA0" };
            game.AddPrototype(prototypeA0);
            var prototypeB0 = new Entity() { Name = "prototypeB0" };
            game.AddPrototype(prototypeB0);
            var prototypeC0 = new Entity() { Name = "prototypeC0" };
            game.AddPrototype(prototypeC0);

            var prototypeA1 = new Entity() { Name = "prototypeA1" };
            prototypeA1.AddPrototype(prototypeA0);
            game.AddPrototype(prototypeA1);

            var prototypeB1 = new Entity() { Name = "prototypeB1" };
            prototypeB1.AddPrototype(prototypeB0);
            prototypeB1.AddPrototype(prototypeC0);
            game.AddPrototype(prototypeB1);

            var scene = new Scene("Test Scene");
            game.FirstScene = scene;
            var testEntity = new Entity() { Name = "testEntity" };
            scene.AddEntity(testEntity);
            testEntity.AddPrototype(prototypeA1);
            testEntity.AddPrototype(prototypeB1);

            return game;
        }

        private static Entity GetTestEntity(Game game)
        {
            return game.FirstScene.Entities.First();
        }

        private static Component CreateTestComponent(int val = 5)
        {
            var component = new Component(Workspace.Instance.GetPlugin(typeof(TransformComponent)));
            component.SetProperty("X", new Value(val, true));
            return component;
        }

        [TestMethod]
        public void Property_Local()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            testEntity.AddComponent(CreateTestComponent());

            var property = testEntity.GetComponentByType(typeof(TransformComponent)).GetProperty("X");
            Assert.AreEqual(5, property.Value.GetIntValue());
        }

        [TestMethod]
        public void Property_HighPriority_OneLevel()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeA1 = game.GetPrototype("prototypeA1");
            prototypeA1.AddComponent(CreateTestComponent());

            var property = testEntity.GetComponentByType(typeof(TransformComponent)).GetProperty("X");
            Assert.AreEqual(5, property.Value.GetIntValue());
        }

        [TestMethod]
        public void Property_HighPriority_MultipleLevels()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeA0 = game.GetPrototype("prototypeA0");
            prototypeA0.AddComponent(CreateTestComponent());

            var property = testEntity.GetComponentByType(typeof(TransformComponent)).GetProperty("X");
            Assert.AreEqual(5, property.Value.GetIntValue());
        }

        [TestMethod]
        public void Property_LowPriority_OneLevel()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeB1 = game.GetPrototype("prototypeB1");
            prototypeB1.AddComponent(CreateTestComponent());

            var property = testEntity.GetComponentByType(typeof(TransformComponent)).GetProperty("X");
            Assert.AreEqual(5, property.Value.GetIntValue());
        }

        [TestMethod]
        public void Property_LowPriority_MultipleLevels()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeB0 = game.GetPrototype("prototypeB0");
            prototypeB0.AddComponent(CreateTestComponent());

            var property = testEntity.GetComponentByType(typeof(TransformComponent)).GetProperty("X");
            Assert.AreEqual(5, property.Value.GetIntValue());
        }

        [TestMethod]
        public void Property_LowPriority_OneLevel_Obscured()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeB1 = game.GetPrototype("prototypeB1");
            prototypeB1.AddComponent(CreateTestComponent());

            var prototypeA1 = game.GetPrototype("prototypeA1");
            prototypeA1.AddComponent(CreateTestComponent(10));

            var property = testEntity.GetComponentByType(typeof(TransformComponent)).GetProperty("X");
            Assert.AreEqual(10, property.Value.GetIntValue());
        }

        [TestMethod]
        public void Property_LowPriority_MultipleLevels_Obscured()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeB1 = game.GetPrototype("prototypeB1");
            prototypeB1.AddComponent(CreateTestComponent());

            var prototypeA0 = game.GetPrototype("prototypeA1");
            prototypeA0.AddComponent(CreateTestComponent(10));

            var property = testEntity.GetComponentByType(typeof(TransformComponent)).GetProperty("X");
            Assert.AreEqual(10, property.Value.GetIntValue());
        }

        [TestMethod]
        public void Property_ChangeFromLowToHigh_OneLevel()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeB1 = game.GetPrototype("prototypeB1");
            prototypeB1.AddComponent(CreateTestComponent());

            var prototypeA1 = game.GetPrototype("prototypeA1");
            prototypeA1.AddComponent(CreateTestComponent(10));

            var property = testEntity.GetComponentByType(typeof(TransformComponent)).GetProperty("X");
            Assert.AreEqual(10, property.Value.GetIntValue());

            testEntity.ClearPrototypes();
            testEntity.AddPrototype(prototypeB1);
            testEntity.AddPrototype(prototypeA1);

            property = testEntity.GetComponentByType(typeof(TransformComponent)).GetProperty("X");
            Assert.AreEqual(5, property.Value.GetIntValue());
        }

        [TestMethod]
        public void Property_ChangeFromLowToHigh_MultipleLevels()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeB1 = game.GetPrototype("prototypeB1");
            prototypeB1.AddComponent(CreateTestComponent());

            var prototypeA0 = game.GetPrototype("prototypeA0");
            prototypeA0.AddComponent(CreateTestComponent(10));

            var property = testEntity.GetComponentByType(typeof(TransformComponent)).GetProperty("X");
            Assert.AreEqual(10, property.Value.GetIntValue());

            var prototypeA1 = game.GetPrototype("prototypeA1");

            testEntity.ClearPrototypes();
            testEntity.AddPrototype(prototypeB1);
            testEntity.AddPrototype(prototypeA1);

            property = testEntity.GetComponentByType(typeof(TransformComponent)).GetProperty("X");
            Assert.AreEqual(5, property.Value.GetIntValue());
        }

        [TestMethod]
        public void Property_LowPriority_HighPriorityHasEmptyComponent()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeA1 = game.GetPrototype("prototypeA1");
            prototypeA1.AddComponent(new Component(Workspace.Instance.GetPlugin(typeof(TransformComponent))));

            var prototypeB1 = game.GetPrototype("prototypeB1");
            prototypeB1.AddComponent(CreateTestComponent());

            var property = testEntity.GetComponentByType(typeof(TransformComponent)).GetProperty("X");
            Assert.AreEqual(5, property.Value.GetIntValue());
        }

        [TestMethod]
        public void Property_HighPriority_HighPriorityInherits_LowPriorityHasEmptyComponent()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeA0 = game.GetPrototype("prototypeA0");
            prototypeA0.AddComponent(CreateTestComponent());

            var prototypeB1 = game.GetPrototype("prototypeB1");
            prototypeB1.AddComponent(new Component(Workspace.Instance.GetPlugin(typeof(TransformComponent))));

            var property = testEntity.GetComponentByType(typeof(TransformComponent)).GetProperty("X");
            Assert.AreEqual(5, property.Value.GetIntValue());
        }
    }
}
