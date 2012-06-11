using Kinectitude.Core.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;
using Kinectitude.Tests.Core.TestMocks;
using System;
using System.Collections.Generic;

namespace Kinectitude.Tests.Core.Data
{
    [TestClass]
    public class StringDataTests
    {

        private const string entityTest = "entity test";
        private const string sceneTest = "scene test";
        private const string gameTest = "game test";
        private const string managerTest = "manager test";

        Game game = new Game(new GameLoaderMock());
        Scene scene;
        Entity entity = new Entity(0);
        ComponentMock component = new ComponentMock();
        Event evt = new EventMock();
        ManagerMock manager = new ManagerMock();

        public StringDataTests()
        {
            scene = new Scene(new SceneLoaderMock(new GameLoaderMock()), game);
            game["gtest"] = gameTest;
            scene["stest"] = sceneTest;
            entity["etest"] = entityTest;
            entity["one"] = "1";
            entity.Scene = scene;
            entity.AddComponent(component, "component");
            ClassFactory.RegisterType("component", typeof(ComponentMock));
            evt.Entity = entity;
            ClassFactory.RegisterType("manager", typeof(ManagerMock));
            Tuple<string, string> values = new Tuple<string, string>("Value", managerTest);
            List<Tuple<string, string>> list = new List<Tuple<string,string>>();
            list.Add(values);
            scene.CreateManager("manager", list);
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
                ExpressionReader.CreateExpressionReader("Entity {this.etest} scene {scene.stest}{scene.manager.Value} [this.one + this.one] lol", evt, entity);
            Assert.IsTrue(reader.GetValue() == "Entity " + entityTest + " scene " + sceneTest + managerTest + " 2 lol");
        }

    }
}
