using System.Linq;
using Kinectitude.Editor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kinectitude.Editor.Tests
{
    [TestClass]
    public class SceneTests
    {
        [TestMethod]
        public void SetName()
        {
            Scene scene = new Scene("Test Scene");

            Assert.AreEqual(scene.Name, "Test Scene");
        }

        [TestMethod]
        public void AddAttribute()
        {
            Scene scene = new Scene("Test Scene");
            
            Attribute attribute = new Attribute("test");
            scene.AddAttribute(attribute);

            Assert.AreEqual(1, scene.Attributes.Count(x => x.Key == "test"));
        }

        [TestMethod]
        public void RemoveAttribute()
        {
            Scene scene = new Scene("Test Scene");
            
            Attribute attribute = new Attribute("test");
            scene.AddAttribute(attribute);
            scene.RemoveAttribute(attribute);

            Assert.AreEqual(0, scene.Attributes.Count(x => x.Key == "test"));
        }

        [TestMethod]
        public void AddEntity()
        {
            Scene scene = new Scene("Test Scene");
            
            Entity entity = new Entity();
            scene.AddEntity(entity);

            Assert.AreEqual(1, scene.Entities.Count());
        }

        [TestMethod]
        public void RemoveEntity()
        {
            Scene scene = new Scene("Test Scene");
            
            Entity entity = new Entity();
            scene.AddEntity(entity);
            scene.RemoveEntity(entity);

            Assert.AreEqual(0, scene.Entities.Count());
        }

        [TestMethod]
        public void SceneAttributeCannotInherit()
        {
            Scene scene = new Scene("Test Scene");

            Attribute attribute = new Attribute("test");
            scene.AddAttribute(attribute);

            Assert.IsFalse(attribute.CanInherit);
        }
    }
}
