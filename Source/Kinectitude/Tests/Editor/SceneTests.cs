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
            bool collectionChanged = false;

            Scene scene = new Scene("Test Scene");
            scene.Attributes.CollectionChanged += (sender, e) => collectionChanged = true;

            Attribute attribute = new Attribute("test");
            scene.AddAttribute(attribute);

            Assert.IsTrue(collectionChanged);
            Assert.AreEqual(1, scene.Attributes.Count(x => x.Name == "test"));
        }

        [TestMethod]
        public void RemoveAttribute()
        {
            int eventsFired = 0;

            Scene scene = new Scene("Test Scene");
            scene.Attributes.CollectionChanged += (sender, e) => eventsFired++;

            Attribute attribute = new Attribute("test");
            scene.AddAttribute(attribute);
            scene.RemoveAttribute(attribute);

            Assert.AreEqual(2, eventsFired);
            Assert.AreEqual(0, scene.Attributes.Count(x => x.Name == "test"));
        }

        [TestMethod]
        public void AddEntity()
        {
            bool collectionChanged = false;

            Scene scene = new Scene("Test Scene");
            scene.Entities.CollectionChanged += (o, e) => collectionChanged = true;
            
            Entity entity = new Entity();
            scene.AddEntity(entity);

            Assert.IsTrue(collectionChanged);
            Assert.AreEqual(1, scene.Entities.Count());
        }

        [TestMethod]
        public void RemoveEntity()
        {
            int eventsFired = 0;

            Scene scene = new Scene("Test Scene");
            scene.Entities.CollectionChanged += (o, e) => eventsFired++;

            Entity entity = new Entity();
            scene.AddEntity(entity);
            scene.RemoveEntity(entity);

            Assert.AreEqual(2, eventsFired);
            Assert.AreEqual(0, scene.Entities.Count());
        }

        [TestMethod]
        public void SceneAttributeCannotInherit()
        {
            bool collectionChanged = false;

            Scene scene = new Scene("Test Scene");
            scene.Attributes.CollectionChanged += (o, e) => collectionChanged = true;

            Attribute attribute = new Attribute("test");
            scene.AddAttribute(attribute);

            Assert.IsTrue(collectionChanged);
            //Assert.IsFalse(attribute.CanInherit);
        }
    }
}
