using System.Linq;
using EditorModels.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EditorModels.Tests
{
    [TestClass]
    public class AttributeViewModelTests
    {
        [TestMethod]
        public void SetKey()
        {
            AttributeViewModel attribute = new AttributeViewModel("test");
            attribute.Key = "test2";

            Assert.AreEqual("test2", attribute.Attribute.Key);
        }

        [TestMethod]
        public void SetValue()
        {
            AttributeViewModel attribute = new AttributeViewModel("test");
            attribute.Value = "value";

            Assert.AreEqual("value", attribute.Attribute.Value);
        }

        [TestMethod]
        public void KeyFollowsInheritedAttribute()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            AttributeViewModel parentAttribute = new AttributeViewModel("test");
            parent.AddAttribute(parentAttribute);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            AttributeViewModel childAttribute = child.GetAttribute("test");

            parentAttribute.Key = "test2";

            Assert.IsTrue(childAttribute.IsInherited);
            Assert.AreEqual(0, child.Attributes.Count(x => x.Key == "test"));
            Assert.AreEqual(1, child.Attributes.Count(x => x.Key == "test2"));
        }

        [TestMethod]
        public void ValueFollowsInheritedAttribute()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            AttributeViewModel parentAttribute = new AttributeViewModel("test");
            parent.AddAttribute(parentAttribute);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            AttributeViewModel childAttribute = child.GetAttribute("test");

            parentAttribute.Value = "value";

            Assert.AreEqual("value", childAttribute.Value);
        }

        [TestMethod]
        public void SetInheritedAttributeToLocal()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            AttributeViewModel parentAttribute = new AttributeViewModel("test");
            parent.AddAttribute(parentAttribute);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            Assert.AreEqual(0, child.Entity.Attributes.Count());

            AttributeViewModel childAttribute = child.GetAttribute("test");

            Assert.IsTrue(childAttribute.CanInherit);

            childAttribute.IsInherited = false;
            childAttribute.Value = "value";

            Assert.AreEqual(1, child.Entity.Attributes.Count());
        }

        [TestMethod]
        public void SetLocalAttributeToInherited()
        {
            EntityViewModel parent = new EntityViewModel() { Name = "parent" };
            
            AttributeViewModel parentAttribute = new AttributeViewModel("test");
            parent.AddAttribute(parentAttribute);

            EntityViewModel child = new EntityViewModel();
            child.AddPrototype(parent);

            AttributeViewModel childAttribute = child.GetAttribute("test");

            Assert.IsTrue(childAttribute.CanInherit);
            
            childAttribute.IsInherited = false;
            childAttribute.Value = "value";

            Assert.AreEqual(1, child.Entity.Attributes.Count());

            childAttribute.IsInherited = true;

            Assert.AreEqual(0, child.Entity.Attributes.Count());
        }
    }
}
