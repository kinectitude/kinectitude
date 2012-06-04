using Kinectitude.Core.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;
using Kinectitude.Tests.Core.TestMocks;
using System;

namespace Kinectitude.Tests.Core.Data
{
    [TestClass]
    public class StringDataTests
    {

        private const string entityTest = "entity test";
        private const string sceneTest = "scene test";
        private const string gameTest = "game test";

        static Game game = new Game(new GameLoaderMock());
        static Scene scene = new Scene(new SceneLoaderMock(new GameLoaderMock()), game);
        static Entity entity = new Entity(0);
        static ComponentMock component = new ComponentMock();
        static Event evt = new EventMock();

        static StringDataTests()
        {
            game["gtest"] = gameTest;
            scene["stest"] = sceneTest;
            entity["etest"] = entityTest;
            entity.Scene = scene;
            entity.AddComponent(component, "component");
            ClassFactory.RegisterType("component", typeof(ComponentMock));
            evt.Entity = entity;
        }

        [TestMethod]
        public void ConstantString()
        {
            ExpressionReader reader = ExpressionReader.CreateExpressionReader("constant", null, null);
            Assert.IsTrue(reader.GetValue() == "constant");
        }

        [TestMethod]
        public void BasicReaders()
        {
            ExpressionReader reader = ExpressionReader.CreateExpressionReader("{this.etest}", evt, entity);
            Assert.IsTrue(reader.GetValue() == entityTest);

            reader = ExpressionReader.CreateExpressionReader("{scene.stest}", evt, entity);
            Assert.IsTrue(reader.GetValue() == sceneTest);

            reader = ExpressionReader.CreateExpressionReader("{game.gtest}", evt, entity);
            Assert.IsTrue(reader.GetValue() == gameTest);


            reader = ExpressionReader.CreateExpressionReader("{this.component.I}", evt, entity);
            Assert.IsTrue(reader.GetValue() == component.I.ToString());

        }

        [TestMethod]
        public void MixedReader()
        {
            ExpressionReader reader = 
                ExpressionReader.CreateExpressionReader("Entity {this.etest} scene {scene.stest} lol", evt, entity);
            Assert.IsTrue(reader.GetValue() == "Entity " + entityTest + " scene " + sceneTest + " lol");
        }

    }
}
