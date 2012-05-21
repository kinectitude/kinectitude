using Kinectitude.Editor.Models.Base;
using Kinectitude.Editor.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Attribute = Kinectitude.Editor.Models.Base.Attribute;

namespace Kinectitude.Tests.Editor
{
    [TestClass]
    public class AttributeViewModelTests
    {
        [TestMethod]
        public void AddNewAttributeToGame()
        {
            Game game = new Game(null);
            
            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(game, "test");
            attributeViewModel.Value = "value";
            attributeViewModel.AddAttribute();

            Assert.IsNotNull(game.GetAttribute("test"));
        }

        [TestMethod]
        public void AddNewAttributeToScene()
        {
            Scene scene = new Scene();

            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(scene, "test");
            attributeViewModel.Value = "value";
            attributeViewModel.AddAttribute();

            Assert.IsNotNull(scene.GetAttribute("test"));
        }

        [TestMethod]
        public void RemoveAttributeFromGame()
        {
            Game game = new Game(null);
            game.AddAttribute(new Attribute("test", "value"));

            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(game, "test");
            attributeViewModel.RemoveAttribute();

            Assert.IsNull(game.GetAttribute("test"));
        }

        [TestMethod]
        public void RemoveAttributeFromScene()
        {
            Scene scene = new Scene();
            scene.AddAttribute(new Attribute("test", "value"));

            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(scene, "test");
            attributeViewModel.RemoveAttribute();

            Assert.IsNull(scene.GetAttribute("test"));
        }

        [TestMethod]
        public void GameAttributeCannotInherit()
        {
            Game game = new Game(null);
            game.AddAttribute(new Attribute("test", "value"));

            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(game, "test");

            Assert.IsFalse(attributeViewModel.CanInherit);

            attributeViewModel.IsInherited = true;

            Assert.IsFalse(attributeViewModel.CanInherit);
        }

        [TestMethod]
        public void SceneAttributeCannotInherit()
        {
            Scene scene = new Scene();
            scene.AddAttribute(new Attribute("test", "value"));

            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(scene, "test");

            Assert.IsFalse(attributeViewModel.CanInherit);

            attributeViewModel.IsInherited = true;

            Assert.IsFalse(attributeViewModel.CanInherit);
        }

        [TestMethod]
        public void ExistingGameAttribute()
        {
            Game game = new Game(null);
            game.AddAttribute(new Attribute("test", "value"));

            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(game, "test");

            Assert.IsFalse(attributeViewModel.IsInherited);
            Assert.IsTrue(attributeViewModel.IsLocal);
            Assert.AreEqual(attributeViewModel.Key, "test");
            Assert.AreEqual(attributeViewModel.Value, "value");
            Assert.IsFalse(attributeViewModel.CanInherit);
        }

        [TestMethod]
        public void ExistingSceneAttribute()
        {
            Scene scene = new Scene();
            scene.AddAttribute(new Attribute("test", "value"));

            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(scene, "test");

            Assert.IsFalse(attributeViewModel.IsInherited);
            Assert.IsTrue(attributeViewModel.IsLocal);
            Assert.AreEqual(attributeViewModel.Key, "test");
            Assert.AreEqual(attributeViewModel.Value, "value");
            Assert.IsFalse(attributeViewModel.CanInherit);
        }

        [TestMethod]
        public void ChangeGameAttributeKey()
        {
            Game game = new Game(null);
            game.AddAttribute(new Attribute("test", "value"));

            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(game, "test");
            attributeViewModel.Key = "test2";
            
            Assert.AreEqual(attributeViewModel.Key, "test2");
        }

        [TestMethod]
        public void ChangeSceneAttributeKey()
        {
            Scene scene = new Scene();
            scene.AddAttribute(new Attribute("test", "value"));

            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(scene, "test");
            attributeViewModel.Key = "test2";

            Assert.AreEqual(attributeViewModel.Key, "test2");
        }

        [TestMethod]
        public void ChangeGameAttributeValue()
        {
            Game game = new Game(null);
            game.AddAttribute(new Attribute("test", "value"));

            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(game, "test");
            attributeViewModel.Value = "new value";

            Assert.AreEqual(attributeViewModel.Value, "new value");
        }

        [TestMethod]
        public void ChangeSceneAttributeValue()
        {
            Scene scene = new Scene();
            scene.AddAttribute(new Attribute("test", "value"));

            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(scene, "test");
            attributeViewModel.Value = "new value";

            Assert.AreEqual(attributeViewModel.Value, "new value");
        }
    }
}
