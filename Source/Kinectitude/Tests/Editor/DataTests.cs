using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models;
using Kinectitude.Core.Data;
using Attribute = Kinectitude.Editor.Models.Attribute;
using Kinectitude.Core.Components;
using Kinectitude.Editor.Models.Values;
using Kinectitude.Core.Managers;
using Kinectitude.Core.Attributes;

namespace Kinectitude.Tests.Editor
{
    [TestClass]
    public class DataTests
    {
        private sealed class DataListener : IChanges
        {
            private bool prepared;
            private bool changed;

            public DataListener()
            {
                prepared = false;
                changed = false;
            }

            public void Prepare()
            {
                prepared = true;
            }

            public void Change()
            {
                changed = true;
            }

            public void Validate()
            {
                Assert.IsTrue(prepared && changed);
            }
        }

        private enum ShapeType
        {
            Rectangle,
            Ellipse
        }

        [Plugin("Mock Render Component", "")]
        private sealed class RenderComponent : Kinectitude.Core.Base.Component
        {
            [PluginProperty("Shape", "")]
            public ShapeType Shape { get; set; }

            public override void Destroy() { }
        }

        [Plugin("Mock Physics Manager", "")]
        private sealed class PhysicsManager : Kinectitude.Core.Base.Manager<Kinectitude.Core.Base.Component>
        {
            [PluginProperty("X Gravity", "")]
            public double XGravity { get; set; }
        
            public override void OnUpdate(float frameDelta) { }
        }

        [Plugin("Mock Service", "")]
        private sealed class MockService : Kinectitude.Core.Base.Service
        {
            [PluginProperty("Mock Property", "")]
            public int MockProperty { get; set; }

            public override void OnStart() { }

            public override void OnStop() { }

            public override bool AutoStart() { return false; }
        }


        private static readonly int TestInteger = 5;
        private static readonly int DefaultInteger = ConstantReader.NullValue.GetIntValue();

        private static readonly string TransformShort = "Transform";
        private static readonly string PhysicsManagerShort = "PhysicsManager";
        private static readonly string MockServiceShort = "MockService";
        private static readonly string MockServiceType = typeof(MockService).FullName;
        private static readonly string TransformComponentType = typeof(TransformComponent).FullName;
        private static readonly string PhysicsManagerType = typeof(PhysicsManager).FullName;
        private static readonly string TimeManagerType = typeof(TimeManager).FullName;

        private static Game CreateTestGame()
        {
            var game = new Game("Test Game");

            var scene = new Scene("Main");
            game.AddScene(scene);

            var entity = new Entity();
            scene.AddEntity(entity);

            return game;
        }

        private static Scene GetTestScene(Game game)
        {
            return game.Scenes.First();
        }

        private static Entity GetTestEntity(Game game)
        {
            return GetTestScene(game).Entities.First();
        }

        public DataTests()
        {
            Workspace.Instance.AddPlugin(new Plugin(typeof(RenderComponent)));
            Workspace.Instance.AddPlugin(new Plugin(typeof(PhysicsManager)));
            Workspace.Instance.AddPlugin(new Plugin(typeof(MockService)));
        }

        private static Attribute CreateTestAttribute()
        {
            return new Attribute("test") { Value = new Value(TestInteger, true) };
        }

        private static Attribute CreateReaderAttribute(string initializer)
        {
            return new Attribute("reader") { Value = new Value(initializer) };
        }

        //An attribute x from This

        // The owning entity changes
        [TestMethod]
        public void This_EntityChanged()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            
            var entity1 = GetTestEntity(game);
            
            var entity2 = new Entity();
            scene.AddEntity(entity2);

            entity1.AddAttribute(CreateTestAttribute());

            var readerAttr = CreateReaderAttribute("test");
            entity1.AddAttribute(readerAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            entity1.RemoveAttribute(readerAttr);
            entity2.AddAttribute(readerAttr);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }
        
        // The attribute x is added to this
        [TestMethod]
        public void This_AttributeAdded()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);

            var readerAttr = CreateReaderAttribute("test");
            entity.AddAttribute(readerAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            entity.AddAttribute(CreateTestAttribute());

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        // The attribute x is removed from this
        [TestMethod]
        public void This_AttributeRemoved()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);

            var readerAttr = CreateReaderAttribute("test");
            entity.AddAttribute(readerAttr);

            var testAttr = CreateTestAttribute();
            entity.AddAttribute(testAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            entity.RemoveAttribute(testAttr);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The attribute x is changed
        [TestMethod]
        public void This_AttributeChanged()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);

            var readerAttr = CreateReaderAttribute("test");
            entity.AddAttribute(readerAttr);

            var testAttr = CreateTestAttribute();
            entity.AddAttribute(testAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            testAttr.Value = new Value(10, true);

            listener.Validate();
            Assert.AreEqual(10, readerAttr.Value.GetIntValue());
        }

        //    The owning entity inherits from a prototype, and x is added to the prototype
        [TestMethod]
        public void This_AttributeAddedToPrototype()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);
            var prototype = new Entity() { Name = "prototype" };
            game.AddPrototype(prototype);
            entity.AddPrototype(prototype);

            var readerAttr = CreateReaderAttribute("test");
            entity.AddAttribute(readerAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            prototype.AddAttribute(CreateTestAttribute());

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    The owning entity inherits from a prototype, and x is removed from the prototype
        [TestMethod]
        public void This_AttributeRemovedFromPrototype()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);
            var prototype = new Entity() { Name = "prototype" };
            game.AddPrototype(prototype);
            entity.AddPrototype(prototype);

            var readerAttr = CreateReaderAttribute("test");
            entity.AddAttribute(readerAttr);

            var testAttr = CreateTestAttribute();
            prototype.AddAttribute(testAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            prototype.RemoveAttribute(testAttr);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The owning entity inherits from a prototype, and x is changed on the prototype
        [TestMethod]
        public void This_AttributeChangedOnPrototype()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);
            var prototype = new Entity() { Name = "prototype" };
            game.AddPrototype(prototype);
            entity.AddPrototype(prototype);

            var readerAttr = CreateReaderAttribute("test");
            entity.AddAttribute(readerAttr);

            var testAttr = CreateTestAttribute();
            prototype.AddAttribute(testAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            testAttr.Value = new Value(10, true);

            listener.Validate();
            Assert.AreEqual(10, readerAttr.Value.GetIntValue());
        }

        //    The owning entity inherits from a prototype 2 levels up, and x is added to the prototype
        [TestMethod]
        public void This_AttributeAddedToHigherPrototype()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);
            
            var prototype0 = new Entity() { Name = "prototype0" };
            game.AddPrototype(prototype0);
            var prototype1 = new Entity() { Name = "prototype1" };
            game.AddPrototype(prototype1);

            prototype1.AddPrototype(prototype0);
            entity.AddPrototype(prototype1);

            var readerAttr = CreateReaderAttribute("test");
            entity.AddAttribute(readerAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            prototype0.AddAttribute(CreateTestAttribute());

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    The owning entity inherits from a prototype 2 levels up, and x is removed from the prototype
        [TestMethod]
        public void This_AttributeRemovedFromHigherPrototype()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);

            var prototype0 = new Entity() { Name = "prototype0" };
            game.AddPrototype(prototype0);
            var prototype1 = new Entity() { Name = "prototype1" };
            game.AddPrototype(prototype1);

            prototype1.AddPrototype(prototype0);
            entity.AddPrototype(prototype1);

            var readerAttr = CreateReaderAttribute("test");
            entity.AddAttribute(readerAttr);

            var testAttr = CreateTestAttribute();
            prototype0.AddAttribute(testAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            prototype0.RemoveAttribute(testAttr);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The owning entity inherits from a prototype 2 levels up, and x is changed on the prototype
        [TestMethod]
        public void This_AttributeChangedOnHigherPrototype()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);

            var prototype0 = new Entity() { Name = "prototype0" };
            game.AddPrototype(prototype0);
            var prototype1 = new Entity() { Name = "prototype1" };
            game.AddPrototype(prototype1);

            prototype1.AddPrototype(prototype0);
            entity.AddPrototype(prototype1);

            var readerAttr = CreateReaderAttribute("test");
            entity.AddAttribute(readerAttr);

            var testAttr = CreateTestAttribute();
            prototype0.AddAttribute(testAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            testAttr.Value = new Value(10, true);

            listener.Validate();
            Assert.AreEqual(10, readerAttr.Value.GetIntValue());
        }

        //An attribute x from the scene

        //    The owning scene changes
        [TestMethod]
        public void Scene_SceneChanged()
        {
            var game = CreateTestGame();
            var scene1 = GetTestScene(game);

            var scene2 = new Scene("Scene 2");
            game.AddScene(scene2);

            scene1.AddAttribute(CreateTestAttribute());

            var readerAttr = CreateReaderAttribute("scene.test");

            var entity = GetTestEntity(game);
            entity.AddAttribute(readerAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            scene1.RemoveEntity(entity);
            scene2.AddEntity(entity);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The attribute x is added to scene
        [TestMethod]
        public void Scene_AttributeAdded()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);

            var entity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("scene.test");
            entity.AddAttribute(readerAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            scene.AddAttribute(CreateTestAttribute());

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    The attribute x is removed from scene
        [TestMethod]
        public void Scene_AttributeRemoved()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);

            var entity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("scene.test");
            entity.AddAttribute(readerAttr);

            var testAttr = CreateTestAttribute();
            scene.AddAttribute(testAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            scene.RemoveAttribute(testAttr);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The attribute x is changed
        [TestMethod]
        public void Scene_AttributeChanged()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);

            var entity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("scene.test");
            entity.AddAttribute(readerAttr);

            var testAttr = CreateTestAttribute();
            scene.AddAttribute(testAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            testAttr.Value = new Value(10, true);

            listener.Validate();
            Assert.AreEqual(10, readerAttr.Value.GetIntValue());
        }

        //An attribute x from the game

        //    The owning game changes
        //[TestMethod]
        //public void Game_GameChanged()
        //{
            
        //}

        //    The attribute x is added to game
        [TestMethod]
        public void Game_AttributeAdded()
        {
            var game = CreateTestGame();

            var entity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("game.test");
            entity.AddAttribute(readerAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            game.AddAttribute(CreateTestAttribute());

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    The attribute x is removed from game
        [TestMethod]
        public void Game_AttributeRemoved()
        {
            var game = CreateTestGame();

            var entity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("game.test");
            entity.AddAttribute(readerAttr);

            var testAttr = CreateTestAttribute();
            game.AddAttribute(testAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            game.RemoveAttribute(testAttr);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The attribute x is changed
        [TestMethod]
        public void Game_AttributeChanged()
        {
            var game = CreateTestGame();

            var entity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("game.test");
            entity.AddAttribute(readerAttr);

            var testAttr = CreateTestAttribute();
            game.AddAttribute(testAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            testAttr.Value = new Value(10, true);

            listener.Validate();
            Assert.AreEqual(10, readerAttr.Value.GetIntValue());
        }

        //An attribute x from a named entity

        //    The owning scene changes
        [TestMethod]
        public void NamedEntity_SceneChanged()
        {
            var game = CreateTestGame();
            var scene1 = GetTestScene(game);

            var scene2 = new Scene("Scene 2");
            game.AddScene(scene2);

            var testEntity = new Entity() { Name = "testEntity" };
            testEntity.AddAttribute(CreateTestAttribute());
            scene1.AddEntity(testEntity);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.test");
            readerEntity.AddAttribute(readerAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            scene1.RemoveEntity(readerEntity);
            scene2.AddEntity(readerEntity);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }
        
        //    An entity with the name is added
        [TestMethod]
        public void NamedEntity_EntityWithNameAdded()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.test");
            readerEntity.AddAttribute(readerAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            var testEntity = new Entity() { Name = "testEntity" };
            testEntity.AddAttribute(CreateTestAttribute());
            scene.AddEntity(testEntity);

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    An entity with the name is removed
        [TestMethod]
        public void NamedEntity_EntityWithNameRemoved()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.test");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            testEntity.AddAttribute(CreateTestAttribute());
            scene.AddEntity(testEntity);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            scene.RemoveEntity(testEntity);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    An entity is renamed to the name
        [TestMethod]
        public void NamedEntity_EntityRenamedToName()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.test");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "someEntity" };
            testEntity.AddAttribute(CreateTestAttribute());
            scene.AddEntity(testEntity);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            testEntity.Name = "testEntity";

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    The entity exists, and x is added to it
        [TestMethod]
        public void NamedEntity_EntityExists_AttributeAdded()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.test");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            scene.AddEntity(testEntity);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            testEntity.AddAttribute(CreateTestAttribute());

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    The entity exists, and x is removed from it
        [TestMethod]
        public void NamedEntity_EntityExists_AttributeRemoved()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.test");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            scene.AddEntity(testEntity);

            var testAttr = CreateTestAttribute();
            testEntity.AddAttribute(testAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            testEntity.RemoveAttribute(testAttr);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The entity exists, and x changes on it
        [TestMethod]
        public void NamedEntity_EntityExists_AttributeChanged()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.test");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            scene.AddEntity(testEntity);

            var testAttr = CreateTestAttribute();
            testEntity.AddAttribute(testAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            testAttr.Value = new Value(10, true);

            listener.Validate();
            Assert.AreEqual(10, readerAttr.Value.GetIntValue());
        }

        //    The entity inherits from prototype, and x is added to the prototype
        [TestMethod]
        public void NamedEntity_EntityExists_AttributeAddedToPrototype()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            var prototype = new Entity() { Name = "prototype" };
            game.AddPrototype(prototype);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.test");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            testEntity.AddPrototype(prototype);
            scene.AddEntity(testEntity);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            var testAttr = CreateTestAttribute();
            prototype.AddAttribute(testAttr);

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    The entity inherits from prototype, and x is removed from the prototype
        [TestMethod]
        public void NamedEntity_EntityExists_AttributeRemovedFromPrototype()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            var prototype = new Entity() { Name = "prototype" };
            game.AddPrototype(prototype);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.test");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            testEntity.AddPrototype(prototype);
            scene.AddEntity(testEntity);

            var testAttr = CreateTestAttribute();
            prototype.AddAttribute(testAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            prototype.RemoveAttribute(testAttr);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The entity inherits from prototype, and x is changed on the prototype
        [TestMethod]
        public void NamedEntity_EntityExists_AttributeChangedOnPrototype()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            var prototype = new Entity() { Name = "prototype" };
            game.AddPrototype(prototype);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.test");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            testEntity.AddPrototype(prototype);
            scene.AddEntity(testEntity);

            var testAttr = CreateTestAttribute();
            prototype.AddAttribute(testAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            testAttr.Value = new Value(10, true);

            listener.Validate();
            Assert.AreEqual(10, readerAttr.Value.GetIntValue());
        }

        //    The entity inherits from prototype 2 levels up, and x is added to the prototype
        [TestMethod]
        public void NamedEntity_EntityExists_AttributeAddedToHigherPrototype()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            
            var prototype0 = new Entity() { Name = "prototype0" };
            game.AddPrototype(prototype0);
            var prototype1 = new Entity() { Name = "prototype1" };
            game.AddPrototype(prototype1);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.test");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            prototype1.AddPrototype(prototype0);
            testEntity.AddPrototype(prototype1);
            scene.AddEntity(testEntity);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            var testAttr = CreateTestAttribute();
            prototype0.AddAttribute(testAttr);

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    The entity inherits from prototype 2 levels up, and x is removed from the prototype
        [TestMethod]
        public void NamedEntity_EntityExists_AttributeRemovedFromHigherPrototype()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);

            var prototype0 = new Entity() { Name = "prototype0" };
            game.AddPrototype(prototype0);
            var prototype1 = new Entity() { Name = "prototype1" };
            game.AddPrototype(prototype1);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.test");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            prototype1.AddPrototype(prototype0);
            testEntity.AddPrototype(prototype1);
            scene.AddEntity(testEntity);

            var testAttr = CreateTestAttribute();
            prototype0.AddAttribute(testAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            prototype0.RemoveAttribute(testAttr);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The entity inherits from prototype 2 levels up, and x is changed on the prototype
        [TestMethod]
        public void NamedEntity_EntityExists_AttributeChangedOnHigherPrototype()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);

            var prototype0 = new Entity() { Name = "prototype0" };
            game.AddPrototype(prototype0);
            var prototype1 = new Entity() { Name = "prototype1" };
            game.AddPrototype(prototype1);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.test");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            prototype1.AddPrototype(prototype0);
            testEntity.AddPrototype(prototype1);
            scene.AddEntity(testEntity);

            var testAttr = CreateTestAttribute();
            prototype0.AddAttribute(testAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            testAttr.Value = new Value(10, true);

            listener.Validate();
            Assert.AreEqual(10, readerAttr.Value.GetIntValue());
        }

        //A component property X.Y from This

        private static Component CreateTestComponent()
        {
            var component = new Component(Workspace.Instance.GetPlugin(TransformComponentType));
            component.SetProperty("X", new Value(TestInteger, true));

            return component;
        }

        //    The owning game changes (since this affects definitions)
        
        //    The definition X is added
        [TestMethod]
        public void ThisComponent_DefinitionAdded()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);

            var readerAttr = CreateReaderAttribute("Transform.X");
            entity.AddAttribute(readerAttr);

            entity.AddComponent(CreateTestComponent());

            var use = game.Usings.Single();
            var define = use.Defines.Single();
            define.Name = "Transform";
            use.RemoveDefine(define);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            use.AddDefine(define);

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    The definition X is removed
        [TestMethod]
        public void ThisComponent_DefinitionRemoved()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);

            var readerAttr = CreateReaderAttribute("Transform.X");
            entity.AddAttribute(readerAttr);

            entity.AddComponent(CreateTestComponent());

            var use = game.Usings.Single();
            var define = use.Defines.Single();
            define.Name = "Transform";

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            use.RemoveDefine(define);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The definition of X changes
        [TestMethod]
        public void ThisComponent_DefinitionChanged()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);

            var readerAttr = CreateReaderAttribute("Transform.X");
            entity.AddAttribute(readerAttr);

            entity.AddComponent(CreateTestComponent());

            var use = game.Usings.Single();
            var define = use.Defines.Single();
            
            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            define.Name = "Transform";

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    The owning entity changes
        [TestMethod]
        public void ThisComponent_OwningEntityChanged()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            var entity1 = GetTestEntity(game);
            
            var entity2 = new Entity();
            entity2.AddComponent(CreateTestComponent());

            scene.AddEntity(entity2);

            var readerAttr = CreateReaderAttribute("TransformComponent.X");
            entity1.AddAttribute(readerAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            entity1.RemoveAttribute(readerAttr);
            entity2.AddAttribute(readerAttr);

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    A component defined as X is added
        [TestMethod]
        public void ThisComponent_ComponentAdded()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);

            var readerAttr = CreateReaderAttribute("Transform.X");
            entity.AddAttribute(readerAttr);

            var use = new Using();
            var define = new Define(TransformShort, TransformComponentType);
            use.AddDefine(define);
            game.AddUsing(use);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            entity.AddComponent(CreateTestComponent());

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    A component defined as X is removed
        [TestMethod]
        public void ThisComponent_ComponentRemoved()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);

            var readerAttr = CreateReaderAttribute("Transform.X");
            entity.AddAttribute(readerAttr);

            var use = new Using();
            var define = new Define(TransformShort, TransformComponentType);
            use.AddDefine(define);
            game.AddUsing(use);

            var component = CreateTestComponent();
            entity.AddComponent(component);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            entity.RemoveComponent(component);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The entity inherits from prototype, and a component defined as X is added to the prototype
        [TestMethod]
        public void ThisComponent_ComponentAddedToPrototype()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);
            var prototype = new Entity() { Name = "prototype" };
            game.AddPrototype(prototype);

            var readerAttr = CreateReaderAttribute("Transform.X");
            entity.AddAttribute(readerAttr);

            var use = new Using();
            var define = new Define(TransformShort, TransformComponentType);
            use.AddDefine(define);
            game.AddUsing(use);

            entity.AddPrototype(prototype);
            
            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            var component = CreateTestComponent();
            prototype.AddComponent(component);

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    The entity inherits from prototype, and a component defined as X is removed from the prototype
        [TestMethod]
        public void ThisComponent_ComponentRemovedFromPrototype()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);
            var prototype = new Entity() { Name = "prototype" };
            game.AddPrototype(prototype);

            var readerAttr = CreateReaderAttribute("Transform.X");
            entity.AddAttribute(readerAttr);

            var use = new Using();
            var define = new Define(TransformShort, TransformComponentType);
            use.AddDefine(define);
            game.AddUsing(use);

            entity.AddPrototype(prototype);
            var component = CreateTestComponent();
            prototype.AddComponent(component);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            prototype.RemoveComponent(component);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    Component X exists and property Y changes
        [TestMethod]
        public void ThisComponent_ComponentExists_PropertyChanged()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);

            var readerAttr = CreateReaderAttribute("Transform.X");
            entity.AddAttribute(readerAttr);

            var use = new Using();
            var define = new Define(TransformShort, TransformComponentType);
            use.AddDefine(define);
            game.AddUsing(use);

            var component = CreateTestComponent();
            entity.AddComponent(component);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            component.SetProperty("X", new Value(10, true));

            listener.Validate();
            Assert.AreEqual(10, readerAttr.Value.GetIntValue());
        }

        //    Component X exists on a prototype and Y changes
        [TestMethod]
        public void ThisComponent_ComponentExistsOnPrototype_PropertyChanged()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);
            var prototype = new Entity() { Name = "prototype" };
            game.AddPrototype(prototype);

            var readerAttr = CreateReaderAttribute("Transform.X");
            entity.AddAttribute(readerAttr);

            var use = new Using();
            var define = new Define(TransformShort, TransformComponentType);
            use.AddDefine(define);
            game.AddUsing(use);

            entity.AddPrototype(prototype);
            var component = CreateTestComponent();
            prototype.AddComponent(component);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            component.SetProperty("X", new Value(10, true));

            listener.Validate();
            Assert.AreEqual(10, readerAttr.Value.GetIntValue());
        }

        //    Component X exists on a prototype 2 levels up and Y changes
        [TestMethod]
        public void ThisComponent_ComponentExistsOnHigherPrototype_PropertyChanged()
        {
            var game = CreateTestGame();
            var entity = GetTestEntity(game);

            var prototype0 = new Entity() { Name = "prototype0" };
            game.AddPrototype(prototype0);
            var prototype1 = new Entity() { Name = "prototype1" };
            game.AddPrototype(prototype1);

            var readerAttr = CreateReaderAttribute("Transform.X");
            entity.AddAttribute(readerAttr);

            var use = new Using();
            var define = new Define(TransformShort, TransformComponentType);
            use.AddDefine(define);
            game.AddUsing(use);

            prototype1.AddPrototype(prototype0);
            entity.AddPrototype(prototype1);

            var component = CreateTestComponent();
            prototype0.AddComponent(component);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            component.SetProperty("X", new Value(10, true));

            listener.Validate();
            Assert.AreEqual(10, readerAttr.Value.GetIntValue());
        }

        //A manager property X.Y from the scene

        private static Manager CreateTestManager()
        {
            var manager = new Manager(Workspace.Instance.GetPlugin(PhysicsManagerType));
            manager.SetProperty("XGravity", new Value(TestInteger, true));
            return manager;
        }

        //    The owning scene changes
        [TestMethod]
        public void Manager_OwningSceneChanged()
        {
            var game = CreateTestGame();
            var scene1 = GetTestScene(game);
            var entity = GetTestEntity(game);

            var scene2 = new Scene("Scene 2");
            game.AddScene(scene2);

            var readerAttr = CreateReaderAttribute("scene.PhysicsManager.XGravity");
            entity.AddAttribute(readerAttr);

            var use = new Using();
            var define = new Define(PhysicsManagerShort, PhysicsManagerType);
            use.AddDefine(define);
            game.AddUsing(use);

            scene1.AddManager(CreateTestManager());

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            scene1.RemoveEntity(entity);
            scene2.AddEntity(entity);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The owning game changes
        // TODO

        //    A definition X is added
        [TestMethod]
        public void Manager_DefinitionAdded()
        {
            var game = CreateTestGame();

            var scene = GetTestScene(game);
            scene.AddManager(CreateTestManager());

            var entity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("scene.PhysicsManager.XGravity");
            entity.AddAttribute(readerAttr);

            var use = game.Usings.Single();
            var define = use.Defines.Single();
            use.RemoveDefine(define);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            use.AddDefine(define);

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    A definition X is removed
        [TestMethod]
        public void Manager_DefinitionRemoved()
        {
            var game = CreateTestGame();

            var scene = GetTestScene(game);
            scene.AddManager(CreateTestManager());

            var entity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("scene.PhysicsManager.XGravity");
            entity.AddAttribute(readerAttr);

            var use = game.Usings.Single();
            var define = use.Defines.Single();

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            use.RemoveDefine(define);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The definition of X changes
        [TestMethod]
        public void Manager_DefinitionChanged()
        {
            var game = CreateTestGame();

            var scene = GetTestScene(game);
            scene.AddManager(CreateTestManager());

            var entity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("scene.PhysicsManager.XGravity");
            entity.AddAttribute(readerAttr);

            var use = game.Usings.Single();
            var define = use.Defines.Single();
            define.Name = "Physics";

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            define.Name = "PhysicsManager";

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    A manager defined as X is added
        [TestMethod]
        public void Manager_ManagerAdded()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            var entity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("scene.PhysicsManager.XGravity");
            entity.AddAttribute(readerAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            scene.AddManager(CreateTestManager());

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    A manager defined as X is removed
        [TestMethod]
        public void Manager_ManagerRemoved()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            var entity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("scene.PhysicsManager.XGravity");
            entity.AddAttribute(readerAttr);

            var manager = CreateTestManager();
            scene.AddManager(manager);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            scene.RemoveManager(manager);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    Manager X exists and property Y changes
        [TestMethod]
        public void Manager_ManagerExists_PropertyChanged()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            var entity = GetTestEntity(game);

            var readerAttr = CreateReaderAttribute("scene.PhysicsManager.XGravity");
            entity.AddAttribute(readerAttr);

            var manager = CreateTestManager();
            scene.AddManager(manager);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            manager.SetProperty("XGravity", new Value(9.8f, true));

            listener.Validate();
            Assert.AreEqual(9.8f, readerAttr.Value.GetFloatValue());
        }

        //A service property X.Y from the game

        private static Service CreateTestService()
        {
            var service = new Service(Workspace.Instance.GetPlugin(MockServiceType));
            service.SetProperty("MockProperty", new Value(TestInteger, true));
            return service;
        }

        //    The owning game changes

        //    A definition X is added
        [TestMethod]
        public void Service_DefinitionAdded()
        {
            var game = CreateTestGame();
            game.AddService(CreateTestService());
            
            var entity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("game.MockService.MockProperty");
            entity.AddAttribute(readerAttr);
            
            var use = game.Usings.Single();
            var define = use.Defines.Single();
            define.Name = MockServiceShort;
            use.RemoveDefine(define);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            use.AddDefine(define);

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    A definition X is removed
        [TestMethod]
        public void Service_DefinitionRemoved()
        {
            var game = CreateTestGame();
            game.AddService(CreateTestService());

            var entity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("game.MockService.MockProperty");
            entity.AddAttribute(readerAttr);

            var use = game.Usings.Single();
            var define = use.Defines.Single();
            define.Name = MockServiceShort;

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            use.RemoveDefine(define);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The definition of X changes
        [TestMethod]
        public void Service_DefinitionChanged()
        {
            var game = CreateTestGame();
            game.AddService(CreateTestService());

            var entity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("game.MockService.MockProperty");
            entity.AddAttribute(readerAttr);

            var use = game.Usings.Single();
            var define = use.Defines.Single();
            define.Name = "Mock";

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            define.Name = MockServiceShort;

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    A service defined as X is added
        [TestMethod]
        public void Service_ServiceAdded()
        {
            var game = CreateTestGame();
            
            var entity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("game.MockService.MockProperty");
            entity.AddAttribute(readerAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            var service = CreateTestService();
            game.AddService(service);

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    A service defined as X is removed
        [TestMethod]
        public void Service_ServiceRemoved()
        {
            var game = CreateTestGame();

            var entity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("game.MockService.MockProperty");
            entity.AddAttribute(readerAttr);

            var service = CreateTestService();
            game.AddService(service);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            game.RemoveService(service);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    Service X exists and property Y changes
        [TestMethod]
        public void Service_ServiceExists_PropertyChanged()
        {
            var game = CreateTestGame();

            var entity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("game.MockService.MockProperty");
            entity.AddAttribute(readerAttr);

            //var use = game.Usings.Single();
            //var define = use.Defines.Single();
            //define.Name = MockServiceShort;

            var service = CreateTestService();
            game.AddService(service);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            service.SetProperty("MockProperty", new Value(10, true));

            listener.Validate();
            Assert.AreEqual(10, readerAttr.Value.GetIntValue());
        }

        //A component property X.Y from a named entity

        //    The owning game changes (since this affects definitions)
        // TODO
        
        //    the owning scene changes
        [TestMethod]
        public void NamedEntity_Component_OwningSceneChanged()
        {
            var game = CreateTestGame();
            var scene1 = GetTestScene(game);
            
            var testEntity = GetTestEntity(game);
            testEntity.AddComponent(CreateTestComponent());
            testEntity.Name = "testEntity";

            var scene2 = new Scene("Scene 2");
            game.AddScene(scene2);

            var readerEntity = new Entity();
            scene1.AddEntity(readerEntity);

            var readerAttr = CreateReaderAttribute("testEntity.TransformComponent.X");
            readerEntity.AddAttribute(readerAttr);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            scene1.RemoveEntity(readerEntity);
            scene2.AddEntity(readerEntity);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The definition X is added
        [TestMethod]
        public void NamedEntity_Component_DefinitionAdded()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            
            var testEntity = GetTestEntity(game);
            testEntity.AddComponent(CreateTestComponent());
            testEntity.Name = "testEntity";

            var readerEntity = new Entity();
            scene.AddEntity(readerEntity);
            var readerAttr = CreateReaderAttribute("testEntity.Transform.X");
            readerEntity.AddAttribute(readerAttr);

            var use = game.Usings.Single();
            var define = use.Defines.Single();
            use.RemoveDefine(define);
            define.Name = TransformShort;

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            use.AddDefine(define);
            
            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    The definition X is removed
        [TestMethod]
        public void NamedEntity_Component_DefinitionRemoved()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);

            var testEntity = GetTestEntity(game);
            testEntity.AddComponent(CreateTestComponent());
            testEntity.Name = "testEntity";

            var readerEntity = new Entity();
            scene.AddEntity(readerEntity);
            var readerAttr = CreateReaderAttribute("testEntity.Transform.X");
            readerEntity.AddAttribute(readerAttr);

            var use = game.Usings.Single();
            var define = use.Defines.Single();
            define.Name = TransformShort;

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            use.RemoveDefine(define);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The definition of X changes
        [TestMethod]
        public void NamedEntity_Component_DefintionChanged()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);

            var testEntity = GetTestEntity(game);
            testEntity.AddComponent(CreateTestComponent());
            testEntity.Name = "testEntity";

            var readerEntity = new Entity();
            scene.AddEntity(readerEntity);
            var readerAttr = CreateReaderAttribute("testEntity.Transform.X");
            readerEntity.AddAttribute(readerAttr);

            var use = game.Usings.Single();
            var define = use.Defines.Single();

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            define.Name = "Transform";

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    An entity with the name is added
        [TestMethod]
        public void NamedEntity_Component_EntityWithNameAdded()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            var readerEntity = GetTestEntity(game);

            var readerAttr = CreateReaderAttribute("testEntity.TransformComponent.X");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            testEntity.AddComponent(CreateTestComponent());

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            scene.AddEntity(testEntity);

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    An entity with the name is removed
        [TestMethod]
        public void NamedEntity_Component_EntityWithNameRemoved()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            var readerEntity = GetTestEntity(game);

            var readerAttr = CreateReaderAttribute("testEntity.TransformComponent.X");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            testEntity.AddComponent(CreateTestComponent());
            scene.AddEntity(testEntity);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            scene.RemoveEntity(testEntity);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    An entity is renamed to the name
        [TestMethod]
        public void NamedEntity_Component_EntityRenamedToName()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            var readerEntity = GetTestEntity(game);

            var readerAttr = CreateReaderAttribute("testEntity.TransformComponent.X");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "someEntity" };
            testEntity.AddComponent(CreateTestComponent());
            scene.AddEntity(testEntity);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            testEntity.Name = "testEntity";

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    The entity exists, and a component defined as X is added to it
        [TestMethod]
        public void NamedEntity_Component_EntityExists_ComponentAdded()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            var readerEntity = GetTestEntity(game);

            var readerAttr = CreateReaderAttribute("testEntity.TransformComponent.X");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            scene.AddEntity(testEntity);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            testEntity.AddComponent(CreateTestComponent());

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    The entity exists, and a component defined as X is removed
        [TestMethod]
        public void NamedEntity_Component_EntityExists_ComponentRemoved()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            var readerEntity = GetTestEntity(game);

            var readerAttr = CreateReaderAttribute("testEntity.TransformComponent.X");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            scene.AddEntity(testEntity);

            var component = CreateTestComponent();
            testEntity.AddComponent(component);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            testEntity.RemoveComponent(component);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The entity inherits from prototype, and a component defined as X is added to the prototype
        [TestMethod]
        public void NamedEntity_Component_EntityExists_ComponentAddedToPrototype()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            var prototype = new Entity() { Name = "prototype" };
            game.AddPrototype(prototype);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.TransformComponent.X");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            testEntity.AddPrototype(prototype);
            scene.AddEntity(testEntity);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            var component = CreateTestComponent();
            prototype.AddComponent(component);

            listener.Validate();
            Assert.AreEqual(TestInteger, readerAttr.Value.GetIntValue());
        }

        //    The entity inherits from prototype, and a component defined as X is removed from the prototype
        [TestMethod]
        public void NamedEntity_Component_EntityExists_ComponentRemovedFromPrototype()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            var prototype = new Entity() { Name = "prototype" };
            game.AddPrototype(prototype);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.TransformComponent.X");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            testEntity.AddPrototype(prototype);
            scene.AddEntity(testEntity);

            var component = CreateTestComponent();
            prototype.AddComponent(component);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            prototype.RemoveComponent(component);

            listener.Validate();
            Assert.AreEqual(DefaultInteger, readerAttr.Value.GetIntValue());
        }

        //    The entity exists, component X exists, and property Y changes
        [TestMethod]
        public void NamedEntity_Component_EntityExists_ComponentExists_PropertyChanged()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.TransformComponent.X");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            scene.AddEntity(testEntity);

            var component = CreateTestComponent();
            testEntity.AddComponent(component);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            component.SetProperty("X", new Value(10.0d, true));

            listener.Validate();
            Assert.AreEqual(10.0d, readerAttr.Value.GetDoubleValue());
        }

        //    Component X exists on a prototype and Y changes
        [TestMethod]
        public void NamedEntity_Component_EntityExists_ComponentExistsOnPrototype_PropertyChanged()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            var prototype = new Entity() { Name = "prototype" };
            game.AddPrototype(prototype);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.TransformComponent.X");
            readerEntity.AddAttribute(readerAttr);

            var testEntity = new Entity() { Name = "testEntity" };
            testEntity.AddPrototype(prototype);
            scene.AddEntity(testEntity);

            var component = CreateTestComponent();
            prototype.AddComponent(component);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            component.SetProperty("X", new Value(10.0d, true));

            listener.Validate();
            Assert.AreEqual(10.0d, readerAttr.Value.GetDoubleValue());
        }

        //    Component X exists on a prototype 2 levels up and Y changes
        [TestMethod]
        public void NamedEntity_Component_EntityExists_ComponentExistsOnHigherPrototype_PropertyChanged()
        {
            var game = CreateTestGame();
            var scene = GetTestScene(game);
            var prototype0 = new Entity() { Name = "prototype0" };
            game.AddPrototype(prototype0);
            var prototype1 = new Entity() { Name = "prototype1" };
            game.AddPrototype(prototype1);

            var readerEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("testEntity.TransformComponent.X");
            readerEntity.AddAttribute(readerAttr);

            prototype1.AddPrototype(prototype0);

            var testEntity = new Entity() { Name = "testEntity" };
            testEntity.AddPrototype(prototype1);
            scene.AddEntity(testEntity);

            var component = CreateTestComponent();
            prototype0.AddComponent(component);

            var listener = new DataListener();
            readerAttr.Value.Subscribe(listener);

            component.SetProperty("X", new Value(10.0d, true));

            listener.Validate();
            Assert.AreEqual(10.0d, readerAttr.Value.GetDoubleValue());
        }

        [TestMethod]
        public void Value_Int()
        {
            var game = CreateTestGame();
            game.AddAttribute(new Attribute("test") { Value = new Value(5, true) });

            var testEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("game.test");
            testEntity.AddAttribute(readerAttr);

            Assert.AreEqual(5, readerAttr.Value.GetIntValue());
        }

        [TestMethod]
        public void Value_String()
        {
            var game = CreateTestGame();
            game.AddAttribute(new Attribute("test") { Value = new Value("value", true) });

            var testEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("game.test");
            testEntity.AddAttribute(readerAttr);

            Assert.AreEqual("value", readerAttr.Value.GetStringValue());
        }

        [TestMethod]
        public void Value_Enum()
        {
            var game = CreateTestGame();
            var component = new Component(Workspace.Instance.GetPlugin(typeof(RenderComponent)));
            component.SetProperty("Shape", new Value(ShapeType.Ellipse, true));

            var testEntity = GetTestEntity(game);
            testEntity.AddComponent(component);
            var readerAttr = CreateReaderAttribute("RenderComponent.Shape");
            testEntity.AddAttribute(readerAttr);

            Assert.AreEqual(ShapeType.Ellipse, readerAttr.Value.GetEnumValue<ShapeType>());
        }

        [TestMethod]
        public void Value_Float()
        {
            var game = CreateTestGame();
            game.AddAttribute(new Attribute("test") { Value = new Value(16.0f, true) });

            var testEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("game.test");
            testEntity.AddAttribute(readerAttr);

            Assert.AreEqual(16.0f, readerAttr.Value.GetFloatValue());
        }

        [TestMethod]
        public void Value_Double()
        {
            var game = CreateTestGame();
            game.AddAttribute(new Attribute("test") { Value = new Value(32.0d, true) });

            var testEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("game.test");
            testEntity.AddAttribute(readerAttr);

            Assert.AreEqual(32.0d, readerAttr.Value.GetDoubleValue());
        }

        [TestMethod]
        public void Value_Bool()
        {
            var game = CreateTestGame();
            game.AddAttribute(new Attribute("test") { Value = new Value(true, true) });

            var testEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("game.test");
            testEntity.AddAttribute(readerAttr);

            Assert.AreEqual(true, readerAttr.Value.GetBoolValue());
        }

        [TestMethod]
        public void Value_Long()
        {
            var game = CreateTestGame();
            game.AddAttribute(new Attribute("test") { Value = new Value(64L, true) });

            var testEntity = GetTestEntity(game);
            var readerAttr = CreateReaderAttribute("game.test");
            testEntity.AddAttribute(readerAttr);

            Assert.AreEqual(64L, readerAttr.Value.GetLongValue());
        }
    }
}
