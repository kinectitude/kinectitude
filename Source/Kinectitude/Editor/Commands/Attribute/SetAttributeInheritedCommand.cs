using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Attribute
{
    public class SetAttributeInheritedCommand : IUndoableCommand
    {
        private readonly AttributeViewModel attribute;
        private readonly Kinectitude.Editor.Models.Base.Attribute localAttribute;
        private readonly bool shouldInherit;
        private readonly bool wasInherited;

        public string Name
        {
            get { return "Change Attribute Inheritance"; }
        }

        public SetAttributeInheritedCommand(AttributeViewModel attribute, bool shouldInherit)
        {
            this.attribute = attribute;
            this.shouldInherit = shouldInherit;
            wasInherited = attribute.IsInherited;
            localAttribute = attribute.Attribute;

            if (wasInherited)
            {
                localAttribute = new Kinectitude.Editor.Models.Base.Attribute(attribute.InheritedAttribute.Key, attribute.InheritedAttribute.Value);
            }
        }

        public void Execute()
        {
            if (wasInherited != shouldInherit)
            {
                if (!shouldInherit)
                {
                    attribute.Attribute = localAttribute;
                    attribute.Entity.AddAttribute(localAttribute);
                }
                else
                {
                    if (null != attribute.Attribute)
                    {
                        attribute.Entity.RemoveAttribute(attribute.Attribute);
                    }
                    attribute.Attribute = attribute.InheritedAttribute.Attribute;
                }
                attribute.RaisePropertyChanged("Key");
                attribute.RaisePropertyChanged("Value");
                attribute.RaisePropertyChanged("IsInherited");
                attribute.RaisePropertyChanged("IsLocal");
            }
        }

        public void Unexecute()
        {
            
        }
    }
}
