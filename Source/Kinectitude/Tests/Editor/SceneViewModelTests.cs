using System.Linq;
using Kinectitude.Editor.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kinectitude.Editor.Tests
{
    [TestClass]
    public class SceneViewModelTests
    {
        [TestMethod]
        public void SetName()
        {
            SceneViewModel scene = new SceneViewModel("Test Scene");

            Assert.AreEqual(scene.Name, "Test Scene");
        }

        [TestMethod]
        public void AddAttribute()
        {
            SceneViewModel scene = new SceneViewModel("Test Scene");
            
            AttributeViewModel attribute = new AttributeViewModel("test");
            scene.AddAttribute(attribute);

            Assert.AreEqual(1, scene.Attributes.Count(x => x.Key == "test"));
        }

        [TestMethod]
        public void RemoveAttribute()
        {
            SceneViewModel scene = new SceneViewModel("Test Scene");
            
            AttributeViewModel attribute = new AttributeViewModel("test");
            scene.AddAttribute(attribute);
            scene.RemoveAttribute(attribute);

            Assert.AreEqual(0, scene.Attributes.Count(x => x.Key == "test"));
        }

        [TestMethod]
        public void AddEntity()
        {
            SceneViewModel scene = new SceneViewModel("Test Scene");
            
            EntityViewModel entity = new EntityViewModel();
            scene.AddEntity(entity);

            Assert.AreEqual(1, scene.Entities.Count());
        }

        [TestMethod]
        public void RemoveEntity()
        {
            SceneViewModel scene = new SceneViewModel("Test Scene");
            
            EntityViewModel entity = new EntityViewModel();
            scene.AddEntity(entity);
            scene.RemoveEntity(entity);

            Assert.AreEqual(0, scene.Entities.Count());
        }

        [TestMethod]
        public void SceneAttributeCannotInherit()
        {
            SceneViewModel scene = new SceneViewModel("Test Scene");

            AttributeViewModel attribute = new AttributeViewModel("test");
            scene.AddAttribute(attribute);

            Assert.IsFalse(attribute.CanInherit);
        }
    }
}
