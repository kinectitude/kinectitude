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
        public void AddNewLocalAttributeToEntity()
        {
            Entity entity = new Entity();
            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(entity, "test");
            Attribute attribute = entity.GetAttribute("test");

            Assert.IsNull(attribute);

            attributeViewModel.AddAttribute();
            attribute = entity.GetAttribute("test");

            Assert.IsNotNull(attribute);
            Assert.AreEqual(attribute.Key, "test");
        }

        [TestMethod]
        public void RemoveLocalAttributeFromEntity()
        {
            Entity entity = new Entity();
            entity.AddAttribute(new Attribute("test", "value"));

            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(entity, "test");
            attributeViewModel.RemoveAttribute();

            Attribute attribute = entity.GetAttribute("test");

            Assert.IsNull(attribute);
        }

        [TestMethod]
        public void ExistingInheritedAttribute()
        {
            Entity parent = new Entity();
            parent.Name = "parent";
            parent.AddAttribute(new Attribute("test", "value"));
            
            Entity child = new Entity();
            child.AddPrototype(parent);

            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(child, "test");
            
            Assert.IsTrue(attributeViewModel.IsInherited);
            Assert.IsFalse(attributeViewModel.IsLocal);
            Assert.AreEqual(attributeViewModel.Key, "test");
            Assert.AreEqual(attributeViewModel.Value, "value");
            Assert.IsTrue(attributeViewModel.CanInherit);
        }

        [TestMethod]
        public void ExistingLocalNonInheritableAttribute()
        {
            Entity parent = new Entity();
            parent.Name = "parent";
            parent.AddAttribute(new Attribute("test", "value"));

            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(parent, "test");
            
            Assert.IsFalse(attributeViewModel.IsInherited);
            Assert.IsTrue(attributeViewModel.IsLocal);
            Assert.AreEqual(attributeViewModel.Key, "test");
            Assert.AreEqual(attributeViewModel.Value, "value");
            Assert.IsFalse(attributeViewModel.CanInherit);
        }

        [TestMethod]
        public void ExistingLocalInheritableAttribute()
        {
            Entity parent = new Entity();
            parent.Name = "parent";
            parent.AddAttribute(new Attribute("test", "value"));

            Entity child = new Entity();
            child.AddPrototype(parent);
            child.AddAttribute(new Attribute("test", "new value"));

            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(child, "test");
            
            Assert.IsFalse(attributeViewModel.IsInherited);
            Assert.IsTrue(attributeViewModel.IsLocal);
            Assert.AreEqual(attributeViewModel.Key, "test");
            Assert.AreEqual(attributeViewModel.Value, "new value");
            Assert.IsTrue(attributeViewModel.CanInherit);
        }

        [TestMethod]
        public void ChangeLocalAttributeKey()
        {
            Entity entity = new Entity();
            Attribute attribute = new Attribute("test", "value");
            entity.AddAttribute(attribute);

            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(entity, "test");
            attributeViewModel.Key = "changed";

            Assert.AreEqual(attribute.Key, "changed");
        }

        [TestMethod]
        public void ChangeLocalAttributeValue()
        {
            Entity entity = new Entity();
            Attribute attribute = new Attribute("test", "value");
            entity.AddAttribute(attribute);

            AttributeViewModel attributeViewModel = AttributeViewModel.GetViewModel(entity, "test");
            attributeViewModel.Value = "changed";

            Assert.AreEqual(attribute.Value, "changed");
        }

        [TestMethod]
        public void ChangeInheritedAttributeKey()
        {
            Entity parent = new Entity();
            parent.Name = "parent";
            Attribute attribute = new Attribute("test", "value");
            parent.AddAttribute(attribute);

            Entity child = new Entity();
            child.AddPrototype(parent);

            AttributeViewModel parentAttributeViewModel = AttributeViewModel.GetViewModel(parent, "test");
            AttributeViewModel childAttributeViewModel = AttributeViewModel.GetViewModel(child, "test");
            
            childAttributeViewModel.Key = "should_not_change";

            Assert.AreEqual(parentAttributeViewModel.Key, "test");
            Assert.AreEqual(childAttributeViewModel.Key, "test");

            parentAttributeViewModel.Key = "changed";

            Assert.AreEqual(attribute.Key, "changed");
            Assert.AreEqual(childAttributeViewModel.Key, "changed");
        }

        [TestMethod]
        public void ChangeInheritedAttributeValue()
        {
            Entity parent = new Entity();
            parent.Name = "parent";
            Attribute attribute = new Attribute("test", "value");
            parent.AddAttribute(attribute);

            Entity child = new Entity();
            child.AddPrototype(parent);

            AttributeViewModel parentAttributeViewModel = AttributeViewModel.GetViewModel(parent, "test");
            AttributeViewModel childAttributeViewModel = AttributeViewModel.GetViewModel(child, "test");

            childAttributeViewModel.Value = "should_not_change";

            Assert.AreEqual(parentAttributeViewModel.Value, "value");
            Assert.AreEqual(childAttributeViewModel.Value, "value");

            parentAttributeViewModel.Value = "changed";

            Assert.AreEqual(attribute.Value, "changed");
            Assert.AreEqual(childAttributeViewModel.Value, "changed");
        }

        [TestMethod]
        public void ChangeInheritedAttributeToLocal()
        {
            Entity parent = new Entity();
            parent.Name = "parent";
            Attribute attribute = new Attribute("test", "value");
            parent.AddAttribute(attribute);

            Entity child = new Entity();
            child.AddPrototype(parent);

            AttributeViewModel childAttributeViewModel = AttributeViewModel.GetViewModel(child, "test");

            Assert.IsTrue(childAttributeViewModel.IsInherited);
            Assert.IsTrue(childAttributeViewModel.CanInherit);
            Assert.IsNull(child.GetAttribute("test"));

            childAttributeViewModel.IsInherited = false;

            Assert.IsTrue(childAttributeViewModel.IsLocal);
            Assert.IsNotNull(child.GetAttribute("test"));
        }

        [TestMethod]
        public void ChangeLocalAttributeToInherited()
        {
            Entity parent = new Entity();
            parent.Name = "parent";
            Attribute attribute = new Attribute("test", "value");
            parent.AddAttribute(attribute);

            Entity child = new Entity();
            child.AddPrototype(parent);
            child.AddAttribute(new Attribute("test", "new value"));

            AttributeViewModel childAttributeViewModel = AttributeViewModel.GetViewModel(child, "test");

            Assert.IsTrue(childAttributeViewModel.IsLocal);
            Assert.IsTrue(childAttributeViewModel.CanInherit);
            Assert.IsNotNull(child.GetAttribute("test"));

            childAttributeViewModel.IsInherited = true;

            Assert.IsTrue(childAttributeViewModel.IsInherited);
            Assert.IsNull(child.GetAttribute("test"));
        }

        [TestMethod]
        public void AttributeBecomesInheritableAfterParentKeyChange()
        {
            Entity parent = new Entity();
            parent.Name = "parent";
            Attribute attribute = new Attribute("test", "value");
            parent.AddAttribute(attribute);

            Entity child = new Entity();
            child.AddPrototype(parent);
            child.AddAttribute(new Attribute("test2", "new value"));

            AttributeViewModel parentAttributeViewModel = AttributeViewModel.GetViewModel(parent, "test");
            AttributeViewModel childAttributeViewModel = AttributeViewModel.GetViewModel(child, "test2");

            Assert.IsFalse(childAttributeViewModel.CanInherit);

            parentAttributeViewModel.Key = "test2";

            Assert.IsTrue(childAttributeViewModel.CanInherit);
        }

        [TestMethod]
        public void AttributeBecomesInheritableAfterLocalKeyChange()
        {
            Entity parent = new Entity();
            parent.Name = "parent";
            Attribute attribute = new Attribute("test", "value");
            parent.AddAttribute(attribute);

            Entity child = new Entity();
            child.AddPrototype(parent);
            child.AddAttribute(new Attribute("test2", "new value"));

            AttributeViewModel parentAttributeViewModel = AttributeViewModel.GetViewModel(parent, "test");
            AttributeViewModel childAttributeViewModel = AttributeViewModel.GetViewModel(child, "test2");

            Assert.IsFalse(childAttributeViewModel.CanInherit);

            childAttributeViewModel.Key = "test";

            Assert.IsTrue(childAttributeViewModel.CanInherit);
        }

        [TestMethod]
        public void AttributeBecomesInheritableAfterAddToParent()
        {
            Entity parent = new Entity();
            parent.Name = "parent";

            Entity child = new Entity();
            child.AddPrototype(parent);
            child.AddAttribute(new Attribute("test", "value"));

            AttributeViewModel childAttributeViewModel = AttributeViewModel.GetViewModel(child, "test");

            Assert.IsFalse(childAttributeViewModel.CanInherit);

            AttributeViewModel parentAttributeViewModel = AttributeViewModel.GetViewModel(parent, "test");
            parentAttributeViewModel.Value = "new value";
            parentAttributeViewModel.AddAttribute();

            Assert.IsTrue(childAttributeViewModel.CanInherit);
        }

        [TestMethod]
        public void AttributeBecomesInheritableAfterPrototypeChange()
        {
            Entity parent = new Entity();
            parent.Name = "parent";

            Entity parentWithAttribute = new Entity();
            parentWithAttribute.Name = "parentWithAttribute";
            Attribute attribute = new Attribute("test", "new value");
            parentWithAttribute.AddAttribute(attribute);

            Entity child = new Entity();
            child.AddPrototype(parent);
            child.AddAttribute(new Attribute("test", "value"));

            EntityViewModel childEntityViewModel = EntityViewModel.GetViewModel(child);
            AttributeViewModel childAttributeViewModel = AttributeViewModel.GetViewModel(child, "test");

            Assert.IsFalse(childAttributeViewModel.CanInherit);

            childEntityViewModel.AddPrototype(EntityViewModel.GetViewModel(parentWithAttribute));

            Assert.IsTrue(childAttributeViewModel.CanInherit);
        }

        [TestMethod]
        public void AttributeBecomesNonInheritableAfterParentKeyChange()
        {
            Entity parent = new Entity();
            parent.Name = "parent";
            Attribute attribute = new Attribute("test", "value");
            parent.AddAttribute(attribute);

            Entity child = new Entity();
            child.AddPrototype(parent);
            child.AddAttribute(new Attribute("test", "new value"));

            AttributeViewModel parentAttributeViewModel = AttributeViewModel.GetViewModel(parent, "test");
            AttributeViewModel childAttributeViewModel = AttributeViewModel.GetViewModel(child, "test");

            Assert.IsTrue(childAttributeViewModel.CanInherit);
            Assert.IsTrue(childAttributeViewModel.IsLocal);

            parentAttributeViewModel.Key = "test2";

            Assert.AreEqual(childAttributeViewModel.Key, "test");
            Assert.IsFalse(childAttributeViewModel.CanInherit);
        }

        [TestMethod]
        public void AttributeBecomesNonInheritableAfterLocalKeyChange()
        {
            Entity parent = new Entity();
            parent.Name = "parent";
            Attribute attribute = new Attribute("test", "value");
            parent.AddAttribute(attribute);

            Entity child = new Entity();
            child.AddPrototype(parent);
            child.AddAttribute(new Attribute("test", "new value"));

            AttributeViewModel parentAttributeViewModel = AttributeViewModel.GetViewModel(parent, "test");
            AttributeViewModel childAttributeViewModel = AttributeViewModel.GetViewModel(child, "test");

            Assert.IsTrue(childAttributeViewModel.CanInherit);

            childAttributeViewModel.Key = "test2";

            Assert.IsFalse(childAttributeViewModel.CanInherit);
        }

        [TestMethod]
        public void AttributeBecomesNonInheritableAfterRemoveFromParent()
        {
            Entity parent = new Entity();
            parent.Name = "parent";
            Attribute attribute = new Attribute("test", "value");
            parent.AddAttribute(attribute);

            Entity child = new Entity();
            child.AddPrototype(parent);
            child.AddAttribute(new Attribute("test", "new value"));

            AttributeViewModel parentAttributeViewModel = AttributeViewModel.GetViewModel(parent, "test");
            AttributeViewModel childAttributeViewModel = AttributeViewModel.GetViewModel(child, "test");

            Assert.IsTrue(childAttributeViewModel.CanInherit);

            parentAttributeViewModel.RemoveAttribute();

            Assert.IsFalse(childAttributeViewModel.CanInherit);
        }

        [TestMethod]
        public void AttributeBecomesNonInheritableAfterPrototypeChange()
        {
            Entity parent = new Entity();
            parent.Name = "parent";

            Entity parentWithAttribute = new Entity();
            parentWithAttribute.Name = "parentWithAttribute";
            Attribute attribute = new Attribute("test", "new value");
            parentWithAttribute.AddAttribute(attribute);

            Entity child = new Entity();
            child.AddPrototype(parentWithAttribute);
            child.AddAttribute(new Attribute("test", "value"));

            EntityViewModel childEntityViewModel = EntityViewModel.GetViewModel(child);
            AttributeViewModel childAttributeViewModel = AttributeViewModel.GetViewModel(child, "test");

            Assert.IsTrue(childAttributeViewModel.CanInherit);

            childEntityViewModel.RemovePrototype(EntityViewModel.GetViewModel(parentWithAttribute));

            Assert.IsFalse(childAttributeViewModel.CanInherit);
        }

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
