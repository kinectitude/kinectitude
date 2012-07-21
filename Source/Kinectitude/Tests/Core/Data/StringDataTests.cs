using System;
using System.Collections.Generic;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Tests.Core.TestMocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Loaders;

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

        static StringDataTests()
        {
           try
            {
                ClassFactory.RegisterType("component", typeof(ComponentMock));
            }
            catch (ArgumentException)
            {
                //this is incase another test case registered this type already
            }

            try
            {
                ClassFactory.RegisterType("manager", typeof(ManagerMock));
            }
            catch (ArgumentException)
            {
                //this is incase another test case registered this type already
            }
        }

        public StringDataTests()
        {
            scene = new Scene(new SceneLoaderMock(new GameLoaderMock(), new LoaderUtilityMock()), game);
            game["gtest"] = gameTest;
            scene["stest"] = sceneTest;
            entity["etest"] = entityTest;
            entity["one"] = "1";
            entity.Scene = scene;
            
            try
            {
                ClassFactory.RegisterType("component", typeof(ComponentMock));
            }
            catch (ArgumentException)
            {
                //this is incase another test case registered this type already
            }

            try
            {
                ClassFactory.RegisterType("manager", typeof(ManagerMock));
            }
            catch (ArgumentException)
            {
                //this is incase another test case registered this type already
            }

            entity.AddComponent(component, "component");
            evt.Entity = entity;
            Tuple<string, string> values = new Tuple<string, string>("Value", managerTest);
            List<Tuple<string, string>> list = new List<Tuple<string,string>>();
            list.Add(values);
            GameLoaderMock glm = new GameLoaderMock();
            LoadedScene tmp = new LoadedScene("name", new List<Tuple<string,string>>(), new SceneLoaderMock(glm, new LoaderUtilityMock()), glm.Game);
            LoadedManager.GetLoadedManager("manager", tmp, new List<Tuple<string, string>>() { new Tuple<string,string>("Value", managerTest) });
            IManager manager = LoadedManager.CreateManagers(tmp, "manager");
            scene.Managers.Add(manager);
            scene.ManagersDictionary.Add(typeof(ManagerMock), manager);
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
            Assert.IsTrue(reader.GetValue() == "Entity " + entityTest + " scene " + sceneTest + managerTest + " 2 lol", reader.GetValue());
        }

    }
}
