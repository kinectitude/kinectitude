using System;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using Kinectitude.Core.Exceptions;
using Kinectitude.Tests.Core.TestMocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Loaders;
using System.Collections.Generic;

namespace Kinectitude.Tests.Core.Base
{
    [TestClass]
    public class EntityTests
    {
        private const string xmlFile = "sample.xml";

        private static Scene scene;

        static EntityTests()
        {
            try
            {
                ClassFactory.RegisterType("RequiresTransformComponent", typeof(RequiresTransformComponent));
            }
            catch (ArgumentException)
            {
                //this is incase another test case registered this type already
            }
            try
            {
                ClassFactory.RegisterType("component", typeof(ComponentMock));
            }
            catch (ArgumentException)
            {
                //this is incase another test case registered this type already
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MissingRequirementsException))]
        public void MissingComponent()
        {
            LoadedEntity e = new LoadedEntity("", new System.Collections.Generic.List<Tuple<string, string>>(), 0, new List<string>(), new List<string>());
            e.AddLoadedComponent(new LoadedComponent("RequiresTransformComponent", new List<Tuple<string,string>>()));
            GameLoaderMock glm = new GameLoaderMock();
            Scene s = new Scene(new SceneLoaderMock(glm, new LoaderUtilityMock()), glm.Game);
            e.Create(s);
        }

        [TestMethod]
        public void HasComponent()
        {
            LoadedEntity e = new LoadedEntity("", new List<Tuple<string, string>>(), 0, new List<string>(), new List<string>());
            e.AddLoadedComponent(new LoadedComponent("RequiresTransformComponent", new List<Tuple<string,string>>()));
            e.AddLoadedComponent(new LoadedComponent("TransformComponent", new List<Tuple<string, string>>()));
            GameLoaderMock glm = new GameLoaderMock();
            Scene s = new Scene(new SceneLoaderMock(glm, new LoaderUtilityMock()), glm.Game);
            e.Create(s);
        }

        [TestMethod]
        public void DestroyEntity()
        {
            Entity e = new Entity(0);
            string name = "name";
            e.Name = name;
            GameLoaderMock glm = new GameLoaderMock();
            SceneLoaderMock slm = new SceneLoaderMock(glm, new LoaderUtilityMock());
            Scene s = new Scene(slm, new Game(glm));
            s.EntityById[0] = e;
            s.EntityByName[name] = e;
            e.Scene = s;
            ComponentMock cm = new ComponentMock();
            e.AddComponent(cm, "component");
            e.Ready();
            e.Destroy();
            Assert.IsTrue(cm.Destroyed);
            Assert.IsFalse(s.EntityById.ContainsKey(0));
            Assert.IsFalse(s.EntityByName.ContainsKey(name));
        }
    }
}
