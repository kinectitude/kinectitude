using System.Linq;
using EditorModels.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EditorModels.Tests
{
    [TestClass]
    public class EntityViewModelTests
    {
        [TestMethod]
        public void SetName()
        {
            EntityViewModel entity = new EntityViewModel();
            entity.Name = "entity";

            Assert.AreEqual("entity", entity.Name);
        }

        [TestMethod]
        public void AddPrototype()
        {
            EntityViewModel entity = new EntityViewModel();

            EntityViewModel prototype = new EntityViewModel() { Name = "prototype" };
            entity.AddPrototype(prototype);

            Assert.AreEqual(1, entity.Prototypes.Where(x => x.Name == "prototype").Count());
        }

        [TestMethod]
        public void RemovePrototype()
        {
            EntityViewModel entity = new EntityViewModel();
            
            EntityViewModel prototype = new EntityViewModel() { Name = "prototype" };
            entity.AddPrototype(prototype);
            entity.RemovePrototype(prototype);

            Assert.AreEqual(0, entity.Prototypes.Where(x => x.Name == "prototype").Count());
        }

        [TestMethod]
        public void PrototypeMustHaveName()
        {
            EntityViewModel entity = new EntityViewModel();

            EntityViewModel prototype = new EntityViewModel();
            entity.AddPrototype(prototype);

            Assert.AreEqual(0, entity.Prototypes.Count());
        }

        [TestMethod]
        public void AddLocalAttribute()
        {
            EntityViewModel entity = new EntityViewModel();
            
            AttributeViewModel attribute = new AttributeViewModel("test");
            entity.AddAttribute(attribute);

            Assert.AreEqual(1, entity.Attributes.Where(x => x.Key == "test").Count());
        }

        [TestMethod]
        public void EntityInheritsAttributeFromPrototype()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            parent.AddAttribute(new AttributeViewModel("test"));

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            Assert.AreEqual(1, parent.Attributes.Where(x => x.Key == "test").Count());
            Assert.AreEqual(1, child.Attributes.Where(x => x.Key == "test").Count());
        }

        [TestMethod]
        public void RemoveLocalAttribute()
        {
            EntityViewModel entity = new EntityViewModel();
            
            AttributeViewModel attribute = new AttributeViewModel("test");
            entity.AddAttribute(attribute);
            entity.RemoveAttribute(attribute);

            Assert.AreEqual(0, entity.Attributes.Where(x => x.Key == "test").Count());
        }

        [TestMethod]
        public void CannotAddDuplicateAttributeKey()
        {
            EntityViewModel entity = new EntityViewModel();

            entity.AddAttribute(new AttributeViewModel("test"));
            entity.AddAttribute(new AttributeViewModel("test"));

            Assert.AreEqual(1, entity.Attributes.Where(x => x.Key == "test").Count());
        }

        [TestMethod]
        public void CannotRemoveInheritedAttribute()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            parent.AddAttribute(new AttributeViewModel("test"));

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            AttributeViewModel childAttribute = child.GetAttribute("test");
            child.RemoveAttribute(childAttribute);

            Assert.AreEqual(1, child.Attributes.Where(x => x.Key == "test").Count());
        }

        [TestMethod]
        public void AttributeBecomesInheritableAfterParentKeyChange()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            AttributeViewModel attribute = new AttributeViewModel("test") { Value = "value" };
            parent.AddAttribute(attribute);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);
            
            AttributeViewModel childAttribute = new AttributeViewModel("test2") { Value = "new value" };
            child.AddAttribute(childAttribute);

            Assert.IsFalse(childAttribute.CanInherit);

            attribute.Key = "test2";

            Assert.IsTrue(childAttribute.CanInherit);
            Assert.AreEqual(1, parent.Attributes.Count);
            Assert.AreEqual(1, child.Attributes.Count);
        }

        [TestMethod]
        public void CannotSetExistingAttributeKeyToDuplicate()
        {
            EntityViewModel entity = new EntityViewModel();
            entity.AddAttribute(new AttributeViewModel("test1"));

            AttributeViewModel attribute = new AttributeViewModel("test2");
            entity.AddAttribute(attribute);

            attribute.Key = "test1";

            Assert.AreEqual("test2", attribute.Key);
            Assert.AreEqual(2, entity.Attributes.Count);
        }

        [TestMethod]
        public void AttributeBecomesInheritableAfterAddToParent()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);
            
            AttributeViewModel attribute = new AttributeViewModel("test") { Value = "value" };
            child.AddAttribute(attribute);

            Assert.IsFalse(attribute.CanInherit);

            AttributeViewModel parentAttribute = new AttributeViewModel("test") { Value = "new value" };
            parent.AddAttribute(parentAttribute);

            Assert.IsTrue(attribute.CanInherit);
            Assert.AreEqual(1, parent.Attributes.Count);
            Assert.AreEqual(1, child.Attributes.Count);
        }

        [TestMethod]
        public void AttributeBecomesInheritableAfterPrototypeChange()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };

            EntityViewModel parentWithAttribute = new EntityViewModel() { Name = "parentWithAttribute" };
            
            AttributeViewModel attribute = new AttributeViewModel("test") { Value = "new value" };
            parentWithAttribute.AddAttribute(attribute);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);
            
            AttributeViewModel childAttribute = new AttributeViewModel("test") { Value = "value" };
            child.AddAttribute(childAttribute);

            Assert.IsFalse(childAttribute.CanInherit);

            child.AddPrototype(parentWithAttribute);

            Assert.IsTrue(childAttribute.CanInherit);
            Assert.AreEqual(1, parentWithAttribute.Attributes.Count);
            Assert.AreEqual(1, child.Attributes.Count);
        }

        [TestMethod]
        public void AttributesBecomesNonInheritableAfterParentKeyChange()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            AttributeViewModel attribute = new AttributeViewModel("test") { Value = "value" };
            parent.AddAttribute(attribute);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);
            
            AttributeViewModel childAttribute = child.GetAttribute("test");
            childAttribute.IsInherited = false;
            childAttribute.Value = "new value";

            attribute.Key = "test2";

            Assert.AreEqual(childAttribute.Key, "test");
            Assert.IsFalse(childAttribute.CanInherit);
            Assert.AreEqual(1, parent.Attributes.Count);
            Assert.AreEqual(2, child.Attributes.Count);
        }

        [TestMethod]
        public void AttributeBecomesNonInheritableAfterLocalKeyChange()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            AttributeViewModel attribute = new AttributeViewModel("test") { Value = "value" };
            parent.AddAttribute(attribute);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);
            
            AttributeViewModel childAttribute = child.GetAttribute("test");
            childAttribute.IsInherited = false;
            childAttribute.Value = "new value";

            Assert.IsTrue(childAttribute.CanInherit);

            childAttribute.Key = "test2";

            Assert.IsFalse(childAttribute.CanInherit);
            Assert.AreEqual(1, parent.Attributes.Count);
            Assert.AreEqual(2, child.Attributes.Count);
        }

        [TestMethod]
        public void AttributeBecomesNonInheritableAfterRemoveFromParent()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            AttributeViewModel attribute = new AttributeViewModel("test") { Value = "value" };
            parent.AddAttribute(attribute);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            AttributeViewModel childAttribute = child.GetAttribute("test");
            childAttribute.IsInherited = false;
            childAttribute.Value = "new value";

            Assert.IsTrue(childAttribute.CanInherit);

            parent.RemoveAttribute(attribute);

            Assert.IsFalse(childAttribute.CanInherit);
            Assert.AreEqual(0, parent.Attributes.Count);
            Assert.AreEqual(1, child.Attributes.Count);
        }

        [TestMethod]
        public void AttributeBecomesNonInheritableAfterPrototypeChange()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };

            EntityViewModel parentWithAttribute = new EntityViewModel() { Name = "parentWithAttribute" };
            
            AttributeViewModel attribute = new AttributeViewModel("test") { Value = "new value" };
            parentWithAttribute.AddAttribute(attribute);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parentWithAttribute);

            AttributeViewModel childAttribute = child.GetAttribute("test");
            childAttribute.IsInherited = false;
            childAttribute.Value = "value";

            Assert.IsTrue(childAttribute.CanInherit);

            child.RemovePrototype(parentWithAttribute);

            Assert.IsFalse(childAttribute.CanInherit);
            Assert.AreEqual(1, parentWithAttribute.Attributes.Count);
            Assert.AreEqual(1, child.Attributes.Count);
        }

        [TestMethod]
        public void InheritableAttributeExposesParentAttributeAfterKeyChange()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            parent.AddAttribute(new AttributeViewModel("test") { Value = "value" });

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            Assert.AreEqual(child.Attributes.Count, 1);

            AttributeViewModel childAttribute = child.GetAttribute("test");
            childAttribute.IsInherited = false;
            childAttribute.Key = "test2";

            Assert.AreEqual(child.Attributes.Count, 2);
            Assert.IsNotNull(child.GetAttribute("test"));
            Assert.IsNotNull(child.GetAttribute("test2"));
        }

        [TestMethod]
        public void ParentAttributeAvailableInChildAfterKeyChange()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            AttributeViewModel parentAttribute = new AttributeViewModel("test") { Value = "value" };
            parent.AddAttribute(parentAttribute);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            AttributeViewModel childAttribute = child.GetAttribute("test");
            childAttribute.IsInherited = false;
            parentAttribute.Key = "test2";

            Assert.AreEqual(child.Attributes.Count, 2);
            Assert.IsNotNull(child.GetAttribute("test"));
            Assert.IsNotNull(child.GetAttribute("test2"));
        }

        [TestMethod]
        public void AddEvent()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void RemoveEvent()
        {
            Assert.Fail();
        }

        /*[TestMethod]
        public void EntityFollowsPrototypeNameChange()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            EntityViewModel otherParent = new EntityViewModel() { Name = "otherParent" };
            
            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);
            child.AddPrototype(otherParent);

            Assert.AreEqual("parent", child.Entity.Prototypes.ElementAt(0));
            Assert.AreEqual("otherParent", child.Entity.Prototypes.ElementAt(1));

            parent.Name = "changed";

            Assert.AreEqual("changed", child.Entity.Prototypes.ElementAt(0));
            Assert.AreEqual("otherParent", child.Entity.Prototypes.ElementAt(1));
        }*/

        [TestMethod]
        public void CannotAddDuplicateNameInSameScope()
        {
            GameViewModel game = new GameViewModel("Test Game");
            game.AddPrototype(new EntityViewModel() { Name = "TestEntity" });

            SceneViewModel scene = new SceneViewModel("Test Scene");
            game.AddScene(scene);

            EntityViewModel entity = new EntityViewModel() { Name = "TestEntity" };
            scene.AddEntity(entity);

            Assert.AreEqual(0, scene.Entities.Count);
        }

        [TestMethod]
        public void AnonymousEntitiesDoNotHaveEqualNames()
        {
            SceneViewModel scene = new SceneViewModel("Test Scene");
            scene.AddEntity(new EntityViewModel());
            scene.AddEntity(new EntityViewModel());

            Assert.AreEqual(2, scene.Entities.Count);
        }

        [TestMethod]
        public void CannotRenameToDuplicateNameInSameScope()
        {
            GameViewModel game = new GameViewModel("Test Game");
            game.AddPrototype(new EntityViewModel() { Name = "TestEntity" });

            SceneViewModel scene = new SceneViewModel("Test Scene");
            
            EntityViewModel entity = new EntityViewModel() { Name = "TestEntity2" };
            scene.AddEntity(entity);
            
            game.AddScene(scene);

            entity.Name = "TestEntity";

            Assert.AreEqual(1, scene.Entities.Count);
            Assert.AreEqual("TestEntity2", entity.Name);
        }

        [TestMethod]
        public void CanAddDuplicateNameInDifferentScope()
        {
            GameViewModel game = new GameViewModel("Test Game");

            SceneViewModel scene1 = new SceneViewModel("Test Scene");

            EntityViewModel entity1 = new EntityViewModel() { Name = "TestEntity" };
            scene1.AddEntity(entity1);

            game.AddScene(scene1);

            SceneViewModel scene2 = new SceneViewModel("Test Scene 2");

            EntityViewModel entity2 = new EntityViewModel() { Name = "TestEntity" };
            scene2.AddEntity(entity2);
            
            game.AddScene(scene2);

            Assert.AreEqual(1, scene1.Entities.Count);
            Assert.AreEqual(1, scene2.Entities.Count);
        }
    }
}
