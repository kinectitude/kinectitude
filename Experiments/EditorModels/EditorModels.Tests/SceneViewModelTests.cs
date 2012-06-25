using System.Linq;
using EditorModels.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EditorModels.Tests
{
    [TestClass]
    public class SceneViewModelTests
    {
        [TestMethod]
        public void SetName()
        {
            SceneViewModel scene = new SceneViewModel("Test Scene");

            Assert.AreEqual(scene.Scene.Name, "Test Scene");
        }

        [TestMethod]
        public void AddAttribute()
        {
            SceneViewModel scene = new SceneViewModel("Test Scene");
            
            AttributeViewModel attribute = new AttributeViewModel("test");
            scene.AddAttribute(attribute);

            Assert.AreEqual(scene.Scene.Attributes.Count(x => x.Key == "test"), 1);
        }

        [TestMethod]
        public void RemoveAttribute()
        {
            SceneViewModel scene = new SceneViewModel("Test Scene");
            
            AttributeViewModel attribute = new AttributeViewModel("test");
            scene.AddAttribute(attribute);
            scene.RemoveAttribute(attribute);

            Assert.AreEqual(scene.Scene.Attributes.Count(x => x.Key == "test"), 0);
        }

        [TestMethod]
        public void AddEntity()
        {
            SceneViewModel scene = new SceneViewModel("Test Scene");
            
            EntityViewModel entity = new EntityViewModel();
            scene.AddEntity(entity);

            Assert.AreEqual(scene.Scene.Entities.Count(), 1);
        }

        [TestMethod]
        public void RemoveEntity()
        {
            SceneViewModel scene = new SceneViewModel("Test Scene");
            
            EntityViewModel entity = new EntityViewModel();
            scene.AddEntity(entity);
            scene.RemoveEntity(entity);

            Assert.AreEqual(scene.Scene.Entities.Count(), 0);
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
