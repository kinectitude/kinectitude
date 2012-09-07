using System.Linq;
using Kinectitude.Editor.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models;

namespace Kinectitude.Editor.Tests
{
    [TestClass]
    public class AttributeTests
    {
        [TestMethod]
        public void SetKey()
        {
            Attribute attribute = new Attribute("test");
            attribute.Key = "test2";

            Assert.AreEqual("test2", attribute.Key);
        }

        [TestMethod]
        public void SetValue()
        {
            Attribute attribute = new Attribute("test");
            attribute.Value = "value";

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

            parentAttribute.Key = "test2";

            Assert.IsTrue(childAttribute.IsInherited);
            Assert.AreEqual(0, child.Attributes.Count(x => x.Key == "test"));
            Assert.AreEqual(1, child.Attributes.Count(x => x.Key == "test2"));
        }

        [TestMethod]
        public void ValueFollowsInheritedAttribute()
        {
            Entity parent = new Entity() { Name = "parent" };
            
            Attribute parentAttribute = new Attribute("test");
            parent.AddAttribute(parentAttribute);

            Entity child = new Entity();
            child.AddPrototype(parent);

            Attribute childAttribute = child.GetAttribute("test");

            parentAttribute.Value = "value";

            Assert.AreEqual("value", childAttribute.Value);
        }

        [TestMethod]
        public void SetInheritedAttributeToLocal()
        {
            Entity parent = new Entity() { Name = "parent" };
            
            Attribute parentAttribute = new Attribute("test");
            parent.AddAttribute(parentAttribute);

            Entity child = new Entity();
            child.AddPrototype(parent);

            Assert.AreEqual(1, child.Attributes.Count(x => x.IsInherited));

            Attribute childAttribute = child.GetAttribute("test");

            Assert.IsTrue(childAttribute.CanInherit);

            childAttribute.IsInherited = false;
            childAttribute.Value = "value";

            Assert.AreEqual(0, child.Attributes.Count(x => x.IsInherited));
        }

        [TestMethod]
        public void SetLocalAttributeToInherited()
        {
            Entity parent = new Entity() { Name = "parent" };
            
            Attribute parentAttribute = new Attribute("test");
            parent.AddAttribute(parentAttribute);

            Entity child = new Entity();
            child.AddPrototype(parent);

            Attribute childAttribute = child.GetAttribute("test");

            Assert.IsTrue(childAttribute.CanInherit);
            
            childAttribute.IsInherited = false;
            childAttribute.Value = "value";

            Assert.AreEqual(0, child.Attributes.Count(x => x.IsInherited));

            childAttribute.IsInherited = true;

            Assert.AreEqual(1, child.Attributes.Count(x => x.IsInherited));
        }
    }
}
