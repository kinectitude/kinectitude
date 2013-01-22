////using System;
////using System.Linq;
////using Microsoft.VisualStudio.TestTools.UnitTesting;
////using Kinectitude.Editor.Models;
////using Kinectitude.Core.Data;
////using Attribute = Kinectitude.Editor.Models.Attribute;
////using Kinectitude.Core.Components;
////using Kinectitude.Core.Base;

////namespace Kinectitude.Tests.Editor
////{
////    [TestClass]
////    public class DataTests
////    {
////        private sealed class DataListener : IChangeable
////        {
////            private bool prepared;
////            private bool changed;

////            public DataListener()
////            {
////                prepared = false;
////                changed = false;
////            }

////            public void Prepare()
////            {
////                prepared = true;
////            }

////            public void Change()
////            {
////                changed = true;
////            }

////            public void Validate()
////            {
////                Assert.IsTrue(prepared && changed);
////            }
////        }

////        private static readonly string TransformShort = "Transform";
////        private static readonly string XProperty = TransformShort + "." + "X";
////        private static readonly string TransformComponentType = typeof(TransformComponent).FullName;

////        private static Game CreateTestGame()
////        {
////            var game = new Game("Test Game");

////            var use = new Using();
////            use.AddDefine(new Define(TransformShort, TransformComponentType));
////            game.AddUsing(use);

////            var scene = new Scene("Main");
////            game.AddScene(scene);

////            var entity = new Entity();
////            scene.AddEntity(entity);

////            return game;
////        }

////        private static Scene GetTestScene(Game game)
////        {
////            return game.Scenes.Single();
////        }

////        private static Entity GetTestEntity(Game game)
////        {
////            return GetTestScene(game).Entities.Single();
////        }

////        [TestMethod]
////        public void ComponentAdded()
////        {
////            var game = CreateTestGame();

////            var entity = GetTestEntity(game);
////            var container = (IDataContainer)entity;

////            var listener = new DataListener();
////            container.NotifyOfComponentChange(Property, listener);

////            entity.AddComponent(new Component(entity.GetPlugin(TransformShort)));

////            listener.Validate();
////        }

////        [TestMethod]
////        public void ComponentRemoved()
////        {
////            var game = CreateTestGame();

////            var entity = GetTestEntity(game);
////            var container = (IDataContainer)entity;

////            var component = new Component(entity.GetPlugin(TransformShort));
////            entity.AddComponent(component);

////            var listener = new DataListener();
////            container.NotifyOfComponentChange(XProperty, listener);

////            entity.RemoveComponent(component);

////            listener.Validate();
////        }

////        [TestMethod]
////        public void LocalPropertyChange()
////        {
////            var game = CreateTestGame();

////            var entity = GetTestEntity(game);
////            var container = (IDataContainer)entity;

////            var component = new Component(entity.GetPlugin(TransformShort));
////            entity.AddComponent(component);

////            var listener = new DataListener();
////            container.NotifyOfComponentChange(XProperty, listener);

////            component.SetProperty("X", 100);

////            listener.Validate();
////        }

////        [TestMethod]
////        public void PrototypePropertyChange()
////        {
////            var game = CreateTestGame();

////            var prototype = new Entity() { Name = "Prototype" };
////            game.AddPrototype(prototype);

////            var component = new Component(prototype.GetPlugin(TransformShort));
////            prototype.AddComponent(component);

////            var entity = GetTestEntity(game);
////            entity.AddPrototype(prototype);

////            var container = (IDataContainer)entity;
////            var listener = new DataListener();
////            container.NotifyOfComponentChange(XProperty, listener);

////            component.SetProperty("X", 100);

////            listener.Validate();
////        }

////        [TestMethod]
////        public void PrototypeChangeAffectsProperty()
////        {
////            var game = CreateTestGame();

////            var prototype = new Entity() { Name = "Prototype" };
////            game.AddPrototype(prototype);

////            var component = new Component(prototype.GetPlugin(TransformShort));
////            component.SetProperty("X", 100);
////            prototype.AddComponent(component);

////            var entity = GetTestEntity(game);
////            entity.AddComponent(new Component(entity.GetPlugin(TransformShort)));

////            var container = (IDataContainer)entity;
////            var listener = new DataListener();
////            container.NotifyOfComponentChange(XProperty, listener);

////            entity.AddPrototype(prototype);

////            listener.Validate();
////        }

////        [TestMethod]
////        public void DefineAdded()
////        {
////            var game = new Game("Test Game");

////            var prototype = new Entity() { Name = "Prototype" };
////            game.AddPrototype(prototype);

////            var container = (IDataContainer)prototype;
////            var listener = new DataListener();
////            container.NotifyOfComponentChange(XProperty, listener);

////            var use = new Using();
////            use.AddDefine(new Define(TransformShort, TransformComponentType));
////            game.AddUsing(use);

////            listener.Validate();
////        }

////        [TestMethod]
////        public void DefineRemoved()
////        {
////            var game = CreateTestGame();

////            var entity = GetTestEntity(game);
////            var container = (IDataContainer)entity;

////            var listener = new DataListener();
////            container.NotifyOfComponentChange(XProperty, listener);

////            game.RemoveUsing(game.Usings.Single());

////            listener.Validate();
////        }

////        [TestMethod]
////        public void DefinedNameChanged()
////        {
////            var game = CreateTestGame();

////            var entity = GetTestEntity(game);
////            var container = (IDataContainer)entity;

////            var listener = new DataListener();
////            container.NotifyOfComponentChange(XProperty, listener);

////            var define = game.Usings.Single().Defines.Single();
////            define.Name = "Transform2";

////            listener.Validate();
////        }

////        [TestMethod]
////        public void GameAttributeAdded()
////        {
////            var game = CreateTestGame();
////            var container = (IDataContainer)game;

////            var listener = new DataListener();
////            container.NotifyOfChange("test", listener);

////            game.AddAttribute(new Attribute("test"));

////            listener.Validate();
////        }

////        [TestMethod]
////        public void GameAttributeRemoved()
////        {
////            var game = CreateTestGame();
////            var container = (IDataContainer)game;

////            var attribute = new Attribute("test");
////            game.AddAttribute(attribute);

////            var listener = new DataListener();
////            container.NotifyOfChange("test", listener);

////            game.RemoveAttribute(attribute);

////            listener.Validate();
////        }

////        [TestMethod]
////        public void GameLocalAttributeChange()
////        {
////            var game = CreateTestGame();
////            var container = (IDataContainer)game;

////            var attribute = new Attribute("test");
////            game.AddAttribute(attribute);

////            var listener = new DataListener();
////            container.NotifyOfChange("test", listener);

////            attribute.Value = new Value("value", null);

////            listener.Validate();
////        }

////        [TestMethod]
////        public void SceneAttributeAdded()
////        {
////            var game = CreateTestGame();
////            var scene = GetTestScene(game);
////            var container = (IDataContainer)scene;

////            var listener = new DataListener();
////            container.NotifyOfChange("test", listener);

////            scene.AddAttribute(new Attribute("test"));

////            listener.Validate();
////        }

////        [TestMethod]
////        public void SceneAttributeRemoved()
////        {
////            var game = CreateTestGame();
////            var scene = GetTestScene(game);
////            var container = (IDataContainer)scene;

////            var attribute = new Attribute("test");
////            scene.AddAttribute(attribute);

////            var listener = new DataListener();
////            container.NotifyOfChange("test", listener);

////            scene.RemoveAttribute(attribute);

////            listener.Validate();
////        }

////        [TestMethod]
////        public void SceneLocalAttributeChange()
////        {
////            var game = CreateTestGame();
////            var scene = GetTestScene(game);
////            var container = (IDataContainer)scene;

////            var attribute = new Attribute("test");
////            scene.AddAttribute(attribute);

////            var listener = new DataListener();
////            container.NotifyOfChange("test", listener);

////            attribute.Value = new Value("value", null);

////            listener.Validate();
////        }

////        [TestMethod]
////        public void EntityAttributeAdded()
////        {
////            var game = CreateTestGame();
////            var entity = GetTestEntity(game);
////            var container = (IDataContainer)entity;

////            var listener = new DataListener();
////            container.NotifyOfChange("test", listener);

////            entity.AddAttribute(new Attribute("test"));

////            listener.Validate();
////        }

////        [TestMethod]
////        public void EntityAttributeRemoved()
////        {
////            var game = CreateTestGame();
////            var entity = GetTestEntity(game);
////            var container = (IDataContainer)entity;

////            var attribute = new Attribute("test");
////            entity.AddAttribute(attribute);

////            var listener = new DataListener();
////            container.NotifyOfChange("test", listener);

////            entity.RemoveAttribute(attribute);

////            listener.Validate();
////        }

////        [TestMethod]
////        public void EntityLocalAttributeChange()
////        {
////            var game = CreateTestGame();
////            var entity = GetTestEntity(game);
////            var container = (IDataContainer)entity;

////            var attribute = new Attribute("test");
////            entity.AddAttribute(attribute);

////            var listener = new DataListener();
////            container.NotifyOfChange("test", listener);

////            attribute.Value = new Value("value", null);

////            listener.Validate();
////        }

////        [TestMethod]
////        public void PrototypeAttributeChange()
////        {
////            var game = CreateTestGame();

////            var prototype = new Entity() { Name = "Prototype" };
////            game.AddPrototype(prototype);

////            var attribute = new Attribute("test");
////            prototype.AddAttribute(attribute);

////            var entity = GetTestEntity(game);
////            entity.AddPrototype(prototype);

////            var container = (IDataContainer)entity;
////            var listener = new DataListener();
////            container.NotifyOfChange("test", listener);

////            attribute.Value = new Value("new value", null);

////            listener.Validate();
////        }

////        [TestMethod]
////        public void PrototypeChangeAffectsAttribute()
////        {
////            var game = CreateTestGame();

////            var prototype = new Entity() { Name = "Prototype" };
////            game.AddPrototype(prototype);

////            var attribute = new Attribute("test");
////            attribute.Value = new Value("value", null);
////            prototype.AddAttribute(attribute);

////            var entity = GetTestEntity(game);
////            entity.AddAttribute(new Attribute("test"));

////            var container = (IDataContainer)entity;
////            var listener = new DataListener();
////            container.NotifyOfChange("test", listener);

////            entity.AddPrototype(prototype);

////            listener.Validate();
////        }
////    }
////}
