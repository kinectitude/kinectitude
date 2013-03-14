using System.Linq;
using Kinectitude.Editor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models.Values;
using Kinectitude.Editor.Models.Exceptions;

namespace Kinectitude.Editor.Tests
{
    [TestClass]
    public class EntityTests
    {
        [TestMethod]
        public void SetName()
        {
            Entity entity = new Entity();
            entity.Name = "entity";

            Assert.AreEqual("entity", entity.Name);
        }

        [TestMethod]
        public void AddPrototype()
        {
            Entity entity = new Entity();

            Entity prototype = new Entity() { Name = "prototype" };
            entity.AddPrototype(prototype);

            Assert.AreEqual(1, entity.Prototypes.Where(x => x.Name == "prototype").Count());
        }

        [TestMethod]
        public void RemovePrototype()
        {
            Entity entity = new Entity();
            
            Entity prototype = new Entity() { Name = "prototype" };
            entity.AddPrototype(prototype);
            entity.RemovePrototype(prototype);

            Assert.AreEqual(0, entity.Prototypes.Where(x => x.Name == "prototype").Count());
        }

        [TestMethod]
        public void PrototypeMustHaveName()
        {
            Entity entity = new Entity();

            Entity prototype = new Entity();
            entity.AddPrototype(prototype);

            Assert.AreEqual(0, entity.Prototypes.Count());
        }

        [TestMethod]
        public void AddLocalAttribute()
        {
            Entity entity = new Entity();
            
            Attribute attribute = new Attribute("test");
            entity.AddAttribute(attribute);

            Assert.AreEqual(1, entity.Attributes.Where(x => x.Name == "test").Count());
        }

        [TestMethod]
        public void EntityInheritsAttributeFromPrototype()
        {
            Entity parent = new Entity() { Name = "parent" };
            parent.AddAttribute(new Attribute("test"));

            Entity child = new Entity();
            child.AddPrototype(parent);

            Assert.AreEqual(1, parent.Attributes.Where(x => x.Name == "test").Count());
            Assert.AreEqual(1, child.Attributes.Where(x => x.Name == "test").Count());
        }

        [TestMethod]
        public void RemoveLocalAttribute()
        {
            Entity entity = new Entity();
            
            Attribute attribute = new Attribute("test");
            entity.AddAttribute(attribute);
            entity.RemoveAttribute(attribute);

            Assert.AreEqual(0, entity.Attributes.Where(x => x.Name == "test").Count());
        }

        [TestMethod, ExpectedException(typeof(AttributeNameExistsException))]
        public void CannotAddDuplicateAttributeKey()
        {
            Entity entity = new Entity();

            entity.AddAttribute(new Attribute("test"));
            entity.AddAttribute(new Attribute("test"));

            Assert.AreEqual(1, entity.Attributes.Where(x => x.Name == "test").Count());
        }

        [TestMethod]
        public void CannotRemoveInheritedAttribute()
        {
            Entity parent = new Entity() { Name = "parent" };
            parent.AddAttribute(new Attribute("test"));

            Entity child = new Entity();
            child.AddPrototype(parent);

            Attribute childAttribute = child.GetAttribute("test");
            child.RemoveAttribute(childAttribute);

            Assert.AreEqual(1, child.Attributes.Where(x => x.Name == "test").Count());
        }

        [TestMethod]
        public void AttributeBecomesInheritableAfterParentKeyChange()
        {
            Entity parent = new Entity() { Name = "parent" };

            Attribute attribute = new Attribute("test") { Value = new Value("value") };
            parent.AddAttribute(attribute);

            Entity child = new Entity();
            child.AddPrototype(parent);

            Attribute childAttribute = new Attribute("test2") { Value = new Value("new value") };
            child.AddAttribute(childAttribute);

            //Assert.IsFalse(childAttribute.CanInherit);

            attribute.Name = "test2";

            //Assert.IsTrue(childAttribute.CanInherit);
            Assert.AreEqual(1, parent.Attributes.Count);
            Assert.AreEqual(1, child.Attributes.Count);
        }

        [TestMethod, ExpectedException(typeof(AttributeNameExistsException))]
        public void CannotSetExistingAttributeKeyToDuplicate()
        {
            Entity entity = new Entity();
            entity.AddAttribute(new Attribute("test1"));

            Attribute attribute = new Attribute("test2");
            entity.AddAttribute(attribute);

            attribute.Name = "test1";

            Assert.AreEqual("test2", attribute.Name);
            Assert.AreEqual(2, entity.Attributes.Count);
        }

        [TestMethod]
        public void AttributeBecomesInheritableAfterAddToParent()
        {
            Entity parent = new Entity() { Name = "parent" };

            Entity child = new Entity();
            child.AddPrototype(parent);

            Attribute attribute = new Attribute("test") { Value = new Value("value") };
            child.AddAttribute(attribute);

            //Assert.IsFalse(attribute.CanInherit);

            Attribute parentAttribute = new Attribute("test") { Value = new Value("new value") };
            parent.AddAttribute(parentAttribute);

            //Assert.IsTrue(attribute.CanInherit);
            Assert.AreEqual(1, parent.Attributes.Count);
            Assert.AreEqual(1, child.Attributes.Count);
        }

        [TestMethod]
        public void AttributeBecomesInheritableAfterPrototypeChange()
        {
            Entity parent = new Entity() { Name = "parent" };

            Entity parentWithAttribute = new Entity() { Name = "parentWithAttribute" };

            Attribute attribute = new Attribute("test") { Value = new Value("new value") };
            parentWithAttribute.AddAttribute(attribute);

            Entity child = new Entity();
            child.AddPrototype(parent);

            Attribute childAttribute = new Attribute("test") { Value = new Value("value") };
            child.AddAttribute(childAttribute);

            //Assert.IsFalse(childAttribute.CanInherit);

            child.AddPrototype(parentWithAttribute);

            //Assert.IsTrue(childAttribute.CanInherit);
            Assert.AreEqual(1, parentWithAttribute.Attributes.Count);
            Assert.AreEqual(1, child.Attributes.Count);
        }

        [TestMethod]
        public void AttributesBecomesNonInheritableAfterParentKeyChange()
        {
            Entity parent = new Entity() { Name = "parent" };

            Attribute attribute = new Attribute("test") { Value = new Value("value") };
            parent.AddAttribute(attribute);

            Entity child = new Entity();
            child.AddPrototype(parent);
            
            Attribute childAttribute = child.GetAttribute("test");
            //childAttribute.IsInherited = false;
            childAttribute.Value = new Value("new value");

            attribute.Name = "test2";

            Assert.AreEqual(childAttribute.Name, "test");
            //Assert.IsFalse(childAttribute.CanInherit);
            Assert.AreEqual(1, parent.Attributes.Count);
            Assert.AreEqual(2, child.Attributes.Count);
        }

        [TestMethod]
        public void CannotChangeKeyForInheritedAttribute()
        {
            Entity parent = new Entity() { Name = "parent" };

            Attribute attribute = new Attribute("test") { Value = new Value("value") };
            parent.AddAttribute(attribute);

            Entity child = new Entity();
            child.AddPrototype(parent);
            
            Attribute childAttribute = child.GetAttribute("test");
            childAttribute.Value = new Value("new value");

            Assert.IsTrue(childAttribute.IsInherited);
            Assert.IsTrue(childAttribute.Name == "test");

            childAttribute.Name = "test2";

            Assert.IsTrue(childAttribute.IsInherited);
            Assert.IsTrue(childAttribute.Name == "test");
            Assert.AreEqual(1, parent.Attributes.Count);
            Assert.AreEqual(1, child.Attributes.Count);
        }

        [TestMethod]
        public void AttributeBecomesNonInheritableAfterRemoveFromParent()
        {
            Entity parent = new Entity() { Name = "parent" };

            Attribute attribute = new Attribute("test") { Value = new Value("value") };
            parent.AddAttribute(attribute);

            Entity child = new Entity();
            child.AddPrototype(parent);

            Attribute childAttribute = child.GetAttribute("test");
            //childAttribute.IsInherited = false;
            childAttribute.Value = new Value("new value");

            //Assert.IsTrue(childAttribute.CanInherit);

            parent.RemoveAttribute(attribute);

            //Assert.IsFalse(childAttribute.CanInherit);
            Assert.AreEqual(0, parent.Attributes.Count);
            Assert.AreEqual(1, child.Attributes.Count);
        }

        [TestMethod]
        public void AttributeBecomesNonInheritableAfterPrototypeChange()
        {
            Entity parent = new Entity() { Name = "parent" };

            Entity parentWithAttribute = new Entity() { Name = "parentWithAttribute" };

            Attribute attribute = new Attribute("test") { Value = new Value("new value") };
            parentWithAttribute.AddAttribute(attribute);

            Entity child = new Entity();
            child.AddPrototype(parentWithAttribute);

            Attribute childAttribute = child.GetAttribute("test");
            //childAttribute.IsInherited = false;
            childAttribute.Value = new Value("value");

            //Assert.IsTrue(childAttribute.CanInherit);

            child.RemovePrototype(parentWithAttribute);

            //Assert.IsFalse(childAttribute.CanInherit);
            Assert.AreEqual(1, parentWithAttribute.Attributes.Count);
            Assert.AreEqual(1, child.Attributes.Count);
        }

        //[TestMethod]
        //public void InheritableAttributeExposesParentAttributeAfterKeyChange()
        //{
        //    Entity parent = new Entity() { Name = "parent" };
        //    parent.AddAttribute(new Attribute("test") { Value = "value" });

        //    Entity child = new Entity();
        //    child.AddPrototype(parent);

        //    Assert.AreEqual(child.Attributes.Count, 1);

        //    Attribute childAttribute = child.GetAttribute("test");
        //    //childAttribute.IsInherited = false;
        //    childAttribute.Name = "test2";

        //    Assert.AreEqual(2, child.Attributes.Count);
        //    Assert.IsNotNull(child.GetAttribute("test"));
        //    Assert.IsNotNull(child.GetAttribute("test2"));
        //}

        //[TestMethod]
        //public void ParentAttributeAvailableInChildAfterKeyChange()
        //{
        //    Entity parent = new Entity() { Name = "parent" };
            
        //    Attribute parentAttribute = new Attribute("test") { Value = "value" };
        //    parent.AddAttribute(parentAttribute);

        //    Entity child = new Entity();
        //    child.AddPrototype(parent);

        //    Attribute childAttribute = child.GetAttribute("test");
        //    //childAttribute.IsInherited = false;
        //    parentAttribute.Name = "test2";

        //    Assert.AreEqual(child.Attributes.Count, 2);
        //    Assert.IsNotNull(child.GetAttribute("test"));
        //    Assert.IsNotNull(child.GetAttribute("test2"));
        //}

        [TestMethod, ExpectedException(typeof(EntityNameExistsException))]
        public void CannotAddDuplicateNameInSameScope()
        {
            Game game = new Game("Test Game");
            game.AddPrototype(new Entity() { Name = "TestEntity" });

            Scene scene = new Scene("Test Scene");
            game.AddScene(scene);

            Entity entity = new Entity() { Name = "TestEntity" };
            scene.AddEntity(entity);

            Assert.AreEqual(0, scene.Entities.Count);
        }

        [TestMethod]
        public void AnonymousEntitiesDoNotHaveEqualNames()
        {
            Scene scene = new Scene("Test Scene");
            scene.AddEntity(new Entity());
            scene.AddEntity(new Entity());

            Assert.AreEqual(2, scene.Entities.Count);
        }

        [TestMethod, ExpectedException(typeof(EntityNameExistsException))]
        public void CannotRenameToDuplicateNameInSameScope()
        {
            Game game = new Game("Test Game");
            game.AddPrototype(new Entity() { Name = "TestEntity" });

            Scene scene = new Scene("Test Scene");
            
            Entity entity = new Entity() { Name = "TestEntity2" };
            scene.AddEntity(entity);
            
            game.AddScene(scene);

            entity.Name = "TestEntity";

            Assert.AreEqual(1, scene.Entities.Count);
            Assert.AreEqual("TestEntity2", entity.Name);
        }

        [TestMethod]
        public void CanAddDuplicateNameInDifferentScope()
        {
            Game game = new Game("Test Game");

            Scene scene1 = new Scene("Test Scene");

            Entity entity1 = new Entity() { Name = "TestEntity" };
            scene1.AddEntity(entity1);

            game.AddScene(scene1);

            Scene scene2 = new Scene("Test Scene 2");

            Entity entity2 = new Entity() { Name = "TestEntity" };
            scene2.AddEntity(entity2);
            
            game.AddScene(scene2);

            Assert.AreEqual(1, scene1.Entities.Count);
            Assert.AreEqual(1, scene2.Entities.Count);
        }
    }
}
