using System.Linq;
using Kinectitude.Editor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models.Values;

namespace Kinectitude.Editor.Tests
{
    [TestClass]
    public class AttributeTests
    {
        [TestMethod]
        public void SetKey()
        {
            bool propertyChanged = false;

            Attribute attribute = new Attribute("test");
            attribute.PropertyChanged += (o, e) => propertyChanged = (e.PropertyName == "Key");

            attribute.Name = "test2";

            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("test2", attribute.Name);
        }

        [TestMethod]
        public void SetValue()
        {
            bool propertyChanged = false;

            Attribute attribute = new Attribute("test");
            attribute.PropertyChanged += (o, e) => propertyChanged |= (e.PropertyName == "Value");
            
            attribute.Value = new Value("value");

            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("value", attribute.Value.Initializer);
        }

        [TestMethod]
        public void KeyFollowsInheritedAttribute()
        {
            Entity parent = new Entity() { Name = "parent" };
            
            Attribute parentAttribute = new Attribute("test");
            parent.AddAttribute(parentAttribute);

            Entity child = new Entity();
            child.AddPrototype(parent);

            Attribute childAttribute = child.GetAttribute("test");

            parentAttribute.Name = "test2";

            Assert.IsFalse(childAttribute.HasOwnValue);
            Assert.AreEqual(0, child.Attributes.Count(x => x.Name == "test"));
            Assert.AreEqual(1, child.Attributes.Count(x => x.Name == "test2"));
        }

        [TestMethod]
        public void ValueFollowsInheritedAttribute()
        {
            //bool propertyChanged = false;

            Entity parent = new Entity() { Name = "parent" };
            
            Attribute parentAttribute = new Attribute("test");
            parent.AddAttribute(parentAttribute);

            Entity child = new Entity();
            child.AddPrototype(parent);

            Attribute childAttribute = child.GetAttribute("test");
            //childAttribute.PropertyChanged += (o, e) => propertyChanged = (e.PropertyName == "Value");

            parentAttribute.Value = new Value("value");

            //Assert.IsTrue(propertyChanged);
            Assert.AreEqual("value", childAttribute.Value.Initializer);
        }

        [TestMethod]
        public void GiveInheritedAttributeLocalValue()
        {
            bool propertyChanged = false;

            Entity parent = new Entity() { Name = "parent" };
            
            Attribute parentAttribute = new Attribute("test");
            parent.AddAttribute(parentAttribute);

            Entity child = new Entity();
            child.AddPrototype(parent);

            Assert.AreEqual(1, child.Attributes.Count(x => x.IsInherited && !x.HasOwnValue));

            Attribute childAttribute = child.GetAttribute("test");
            childAttribute.PropertyChanged += (o, e) => propertyChanged |= (e.PropertyName == "HasOwnValue");

            childAttribute.Value = new Value("value");

            Assert.IsTrue(propertyChanged);
            Assert.AreEqual(1, child.Attributes.Count(x => x.HasOwnValue));
        }

        [TestMethod]
        public void ClearAttributeValue()
        {
            Entity parent = new Entity() { Name = "parent" };

            Attribute parentAttribute = new Attribute("test") { Value = new Value("originalValue") };
            parent.AddAttribute(parentAttribute);

            Entity child = new Entity();
            child.AddPrototype(parent);

            Attribute childAttribute = child.GetAttribute("test");

            childAttribute.Value = new Value("value");

            Assert.AreEqual(childAttribute.Value.Initializer, "value");
            Assert.AreEqual(1, child.Attributes.Count(x => x.HasOwnValue));

            childAttribute.Value = null;

            Assert.AreEqual(childAttribute.Value.Initializer, "originalValue");
            Assert.AreEqual(0, child.Attributes.Count(x => x.HasOwnValue));
        }

        // Multiple inheritance tests

        private static Game CreateTestGame()
        {
            var game = new Game("Test Game");

            var prototypeA0 = new Entity() { Name = "prototypeA0" };
            game.AddPrototype(prototypeA0);
            var prototypeB0 = new Entity() { Name = "prototypeB0" };
            game.AddPrototype(prototypeB0);
            var prototypeC0 = new Entity() { Name = "prototypeC0" };
            game.AddPrototype(prototypeC0);

            var prototypeA1 = new Entity() { Name = "prototypeA1" };
            prototypeA1.AddPrototype(prototypeA0);
            game.AddPrototype(prototypeA1);

            var prototypeB1 = new Entity() { Name = "prototypeB1" };
            prototypeB1.AddPrototype(prototypeB0);
            prototypeB1.AddPrototype(prototypeC0);
            game.AddPrototype(prototypeB1);

            var scene = new Scene("Test Scene");
            game.FirstScene = scene;
            var testEntity = new Entity() { Name = "testEntity" };
            scene.AddEntity(testEntity);
            testEntity.AddPrototype(prototypeA1);
            testEntity.AddPrototype(prototypeB1);

            return game;
        }

        private static Entity GetTestEntity(Game game)
        {
            return game.FirstScene.Entities.First();
        }

        private static Attribute CreateTestAttribute(int val = 5)
        {
            return new Attribute("test") { Value = new Value(val, true) };
        }

        [TestMethod]
        public void Attribute_Local()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            testEntity.AddAttribute(CreateTestAttribute());
            var attr = testEntity.GetAttribute("test");

            Assert.AreEqual(5, attr.Value.GetIntValue());
        }

        [TestMethod]
        public void Attribute_HighPriority_OneLevel()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeA1 = game.GetPrototype("prototypeA1");
            prototypeA1.AddAttribute(CreateTestAttribute());

            var attr = testEntity.GetAttribute("test");

            Assert.AreEqual(5, attr.Value.GetIntValue());
        }

        [TestMethod]
        public void Attribute_HighPriority_MultipleLevels()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeA0 = game.GetPrototype("prototypeA0");
            prototypeA0.AddAttribute(CreateTestAttribute());

            var attr = testEntity.GetAttribute("test");

            Assert.AreEqual(5, attr.Value.GetIntValue());
        }

        [TestMethod]
        public void Attribute_LowPriority_OneLevel()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeB1 = game.GetPrototype("prototypeB1");
            prototypeB1.AddAttribute(CreateTestAttribute());

            var attr = testEntity.GetAttribute("test");

            Assert.AreEqual(5, attr.Value.GetIntValue());
        }

        [TestMethod]
        public void Attribute_LowPriority_MultipleLevels()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeB0 = game.GetPrototype("prototypeB0");
            prototypeB0.AddAttribute(CreateTestAttribute());

            var attr = testEntity.GetAttribute("test");

            Assert.AreEqual(5, attr.Value.GetIntValue());
        }

        [TestMethod]
        public void Attribute_LowPriority_OneLevel_Obscured()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeB1 = game.GetPrototype("prototypeB1");
            prototypeB1.AddAttribute(CreateTestAttribute());

            var prototypeA1 = game.GetPrototype("prototypeA1");
            prototypeA1.AddAttribute(CreateTestAttribute(10));

            var attr = testEntity.GetAttribute("test");

            Assert.AreEqual(10, attr.Value.GetIntValue());
        }

        [TestMethod]
        public void Attribute_LowPriority_MultipleLevels_Obscured()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeB1 = game.GetPrototype("prototypeB1");
            prototypeB1.AddAttribute(CreateTestAttribute());

            var prototypeA0 = game.GetPrototype("prototypeA1");
            prototypeA0.AddAttribute(CreateTestAttribute(10));

            var attr = testEntity.GetAttribute("test");

            Assert.AreEqual(10, attr.Value.GetIntValue());
        }

        [TestMethod]
        public void Attribute_ChangeFromLowToHigh_OneLevel()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeB1 = game.GetPrototype("prototypeB1");
            prototypeB1.AddAttribute(CreateTestAttribute());

            var prototypeA1 = game.GetPrototype("prototypeA1");
            prototypeA1.AddAttribute(CreateTestAttribute(10));

            var attr = testEntity.GetAttribute("test");
            Assert.AreEqual(10, attr.Value.GetIntValue());

            testEntity.ClearPrototypes();
            testEntity.AddPrototype(prototypeB1);
            testEntity.AddPrototype(prototypeA1);

            attr = testEntity.GetAttribute("test");
            Assert.AreEqual(5, attr.Value.GetIntValue());
        }

        [TestMethod]
        public void Attribute_ChangeFromLowToHigh_MultipleLevels()
        {
            var game = CreateTestGame();
            var testEntity = GetTestEntity(game);

            var prototypeB1 = game.GetPrototype("prototypeB1");
            prototypeB1.AddAttribute(CreateTestAttribute());

            var prototypeA0 = game.GetPrototype("prototypeA0");
            prototypeA0.AddAttribute(CreateTestAttribute(10));

            var attr = testEntity.GetAttribute("test");
            Assert.AreEqual(10, attr.Value.GetIntValue());

            var prototypeA1 = game.GetPrototype("prototypeA1");

            testEntity.ClearPrototypes();
            testEntity.AddPrototype(prototypeB1);
            testEntity.AddPrototype(prototypeA1);

            attr = testEntity.GetAttribute("test");
            Assert.AreEqual(5, attr.Value.GetIntValue());
        }
    }
}
