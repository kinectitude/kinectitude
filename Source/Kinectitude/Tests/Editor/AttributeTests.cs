using System.Linq;
using Kinectitude.Editor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            
            attribute.Value = "value";

            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("value", attribute.Value);
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

            parentAttribute.Value = "value";

            //Assert.IsTrue(propertyChanged);
            Assert.AreEqual("value", childAttribute.Value);
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

            childAttribute.Value = "value";

            Assert.IsTrue(propertyChanged);
            Assert.AreEqual(1, child.Attributes.Count(x => x.HasOwnValue));
        }

        [TestMethod]
        public void ClearAttributeValue()
        {
            Entity parent = new Entity() { Name = "parent" };

            Attribute parentAttribute = new Attribute("test") { Value = "originalValue" };
            parent.AddAttribute(parentAttribute);

            Entity child = new Entity();
            child.AddPrototype(parent);

            Attribute childAttribute = child.GetAttribute("test");

            childAttribute.Value = "value";

            Assert.AreEqual(childAttribute.Value, "value");
            Assert.AreEqual(1, child.Attributes.Count(x => x.HasOwnValue));

            childAttribute.Value = null;

            Assert.AreEqual(childAttribute.Value, "originalValue");
            Assert.AreEqual(0, child.Attributes.Count(x => x.HasOwnValue));
        }
    }
}
