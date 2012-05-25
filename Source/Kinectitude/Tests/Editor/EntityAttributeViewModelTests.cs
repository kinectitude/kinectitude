using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models.Base;
using Attribute = Kinectitude.Editor.Models.Base.Attribute;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Tests.Editor
{
    [TestClass]
    public class EntityAttributeViewModelTests
    {
        [TestMethod]
        public void AddNewLocalAttributeToEntity()
        {
            Entity entity = new Entity();
            EntityAttributeViewModel attributeViewModel = new EntityAttributeViewModel(entity, "test");
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

            EntityAttributeViewModel attributeViewModel = EntityViewModel.GetViewModel(entity).GetEntityAttributeViewModel("test");
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

            EntityAttributeViewModel attributeViewModel = EntityViewModel.GetViewModel(child).GetEntityAttributeViewModel("test");

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

            EntityAttributeViewModel attributeViewModel = EntityViewModel.GetViewModel(parent).GetEntityAttributeViewModel("test");

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

            EntityAttributeViewModel attributeViewModel = EntityViewModel.GetViewModel(child).GetEntityAttributeViewModel("test");

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

            EntityAttributeViewModel attributeViewModel = EntityViewModel.GetViewModel(entity).GetEntityAttributeViewModel("test");
            attributeViewModel.Key = "changed";

            Assert.AreEqual(attribute.Key, "changed");
        }

        [TestMethod]
        public void ChangeLocalAttributeValue()
        {
            Entity entity = new Entity();
            Attribute attribute = new Attribute("test", "value");
            entity.AddAttribute(attribute);

            EntityAttributeViewModel attributeViewModel = EntityViewModel.GetViewModel(entity).GetEntityAttributeViewModel("test");
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

            EntityAttributeViewModel parentAttributeViewModel = EntityViewModel.GetViewModel(parent).GetEntityAttributeViewModel("test");
            EntityAttributeViewModel childAttributeViewModel = EntityViewModel.GetViewModel(child).GetEntityAttributeViewModel("test");

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

            EntityAttributeViewModel parentAttributeViewModel = EntityViewModel.GetViewModel(parent).GetEntityAttributeViewModel("test");
            EntityAttributeViewModel childAttributeViewModel = EntityViewModel.GetViewModel(child).GetEntityAttributeViewModel("test");

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

            EntityAttributeViewModel childAttributeViewModel = EntityViewModel.GetViewModel(child).GetEntityAttributeViewModel("test");

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

            EntityAttributeViewModel childAttributeViewModel = EntityViewModel.GetViewModel(child).GetEntityAttributeViewModel("test");

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

            EntityAttributeViewModel parentAttributeViewModel = EntityViewModel.GetViewModel(parent).GetEntityAttributeViewModel("test");
            EntityAttributeViewModel childAttributeViewModel = EntityViewModel.GetViewModel(child).GetEntityAttributeViewModel("test2");

            Assert.IsFalse(childAttributeViewModel.CanInherit);

            parentAttributeViewModel.Key = "test2";

            int count = 0;
            for (int i = 0; i < 80; i++)
            {
                count += i;
            }

            //Assert.IsTrue(childAttributeViewModel.CanInherit);
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

            EntityAttributeViewModel parentAttributeViewModel = EntityViewModel.GetViewModel(parent).GetEntityAttributeViewModel("test");
            EntityAttributeViewModel childAttributeViewModel = EntityViewModel.GetViewModel(child).GetEntityAttributeViewModel("test2");

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

            EntityAttributeViewModel childAttributeViewModel = EntityViewModel.GetViewModel(child).GetEntityAttributeViewModel("test");

            Assert.IsFalse(childAttributeViewModel.CanInherit);

            EntityAttributeViewModel parentAttributeViewModel = new EntityAttributeViewModel(parent, "test");
            parentAttributeViewModel.Value = "new value";

            EntityViewModel parentViewModel = EntityViewModel.GetViewModel(parent);
            parentViewModel.AddAttribute(parentAttributeViewModel);

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
            EntityAttributeViewModel childAttributeViewModel = childEntityViewModel.GetEntityAttributeViewModel("test");

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

            EntityAttributeViewModel parentAttributeViewModel = EntityViewModel.GetViewModel(parent).GetEntityAttributeViewModel("test");
            EntityAttributeViewModel childAttributeViewModel = EntityViewModel.GetViewModel(child).GetEntityAttributeViewModel("test");

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

            EntityAttributeViewModel parentAttributeViewModel = EntityViewModel.GetViewModel(parent).GetEntityAttributeViewModel("test");
            EntityAttributeViewModel childAttributeViewModel = EntityViewModel.GetViewModel(child).GetEntityAttributeViewModel("test");

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

            EntityAttributeViewModel parentAttributeViewModel = EntityViewModel.GetViewModel(parent).GetEntityAttributeViewModel("test");
            EntityAttributeViewModel childAttributeViewModel = EntityViewModel.GetViewModel(child).GetEntityAttributeViewModel("test");

            Assert.IsTrue(childAttributeViewModel.CanInherit);

            EntityViewModel.GetViewModel(parent).RemoveAttribute(parentAttributeViewModel);

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
            EntityAttributeViewModel childAttributeViewModel = EntityViewModel.GetViewModel(child).GetEntityAttributeViewModel("test");

            Assert.IsTrue(childAttributeViewModel.CanInherit);

            childEntityViewModel.RemovePrototype(EntityViewModel.GetViewModel(parentWithAttribute));

            Assert.IsFalse(childAttributeViewModel.CanInherit);
        }

        [TestMethod]
        public void InheritableAttributeExposesParentAttributeAfterKeyChange()
        {
            Entity parent = new Entity();
            parent.Name = "parent";
            parent.AddAttribute(new Attribute("test", "value"));

            Entity child = new Entity();
            child.AddPrototype(parent);

            EntityViewModel childEntityViewModel = EntityViewModel.GetViewModel(child);

            Assert.AreEqual(childEntityViewModel.Attributes.Count, 1);

            EntityAttributeViewModel childAttributeViewModel = childEntityViewModel.GetEntityAttributeViewModel("test");
            childAttributeViewModel.IsInherited = false;
            childAttributeViewModel.Key = "test2";

            Assert.AreEqual(childEntityViewModel.Attributes.Count, 2);
            Assert.IsNotNull(childEntityViewModel.GetEntityAttributeViewModel("test"));
            Assert.IsNotNull(childEntityViewModel.GetEntityAttributeViewModel("test2"));
        }

        [TestMethod]
        public void ParentAttributeAvailableInChildAfterKeyChange()
        {
            Entity parent = new Entity();
            parent.Name = "parent";
            parent.AddAttribute(new Attribute("test", "value"));

            Entity child = new Entity();
            child.AddPrototype(parent);

            EntityViewModel childEntityViewModel = EntityViewModel.GetViewModel(child);

            EntityAttributeViewModel parentAttributeViewModel = EntityViewModel.GetViewModel(parent).GetEntityAttributeViewModel("test");
            EntityAttributeViewModel childAttributeViewModel = childEntityViewModel.GetEntityAttributeViewModel("test");

            childAttributeViewModel.IsInherited = false;
            parentAttributeViewModel.Key = "test2";

            Assert.AreEqual(childEntityViewModel.Attributes.Count, 2);
            Assert.IsNotNull(childEntityViewModel.GetEntityAttributeViewModel("test"));
            Assert.IsNotNull(childEntityViewModel.GetEntityAttributeViewModel("test2"));
        }
    }
}
