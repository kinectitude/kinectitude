using System;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using Kinectitude.Core.Exceptions;
using Kinectitude.Tests.Core.TestMocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kinectitude.Tests.Core.Base
{
    [TestClass]
    public class EntityTests
    {

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
            Entity e = new Entity(0);
            e.AddComponent(new RequiresTransformComponent(), "RequiresTransformComponent");
            e.Ready();
        }

        [TestMethod]
        public void HasComponent()
        {
            Entity e = new Entity(0);
            e.AddComponent(new RequiresTransformComponent(), "RequiresTransformComponent");
            e.AddComponent(new TransformComponent(), "TransformComponent");
            e.Ready();
        }

        [TestMethod]
        public void DestroyEntity()
        {
            Entity e = new Entity(0);
            string name = "name";
            e.Name = name;
            GameLoaderMock glm = new GameLoaderMock();
            SceneLoaderMock slm = new SceneLoaderMock(glm);
            slm.EntityById[0] = e;
            slm.EntityByName[name] = e;
            Scene s = new Scene(slm, new Game(glm));
            e.Scene = s;
            ComponentMock cm = new ComponentMock();
            e.AddComponent(cm, "component");
            e.Ready();
            e.Destroy();
            Assert.IsTrue(cm.Destroyed);
            Assert.IsFalse(slm.EntityById.ContainsKey(0));
            Assert.IsFalse(slm.EntityByName.ContainsKey(name));
        }
    }
}
