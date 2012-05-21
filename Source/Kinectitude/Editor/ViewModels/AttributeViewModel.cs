using System.Collections.Generic;
using Kinectitude.Editor.Base;
using Attribute = Kinectitude.Editor.Models.Base.Attribute;
using System.ComponentModel;
using Kinectitude.Editor.Models.Base;
using System.Collections.Specialized;
using Kinectitude.Editor.Commands.Attribute;
using Kinectitude.Editor.Commands.Base;
using System;

namespace Kinectitude.Editor.ViewModels
{
    public class AttributeViewModel : BaseModel, IAttributeViewModel
    {
        private static readonly Dictionary<Tuple<AttributeContainer, string>, AttributeViewModel> attributes;

        static AttributeViewModel()
        {
            attributes = new Dictionary<Tuple<AttributeContainer, string>, AttributeViewModel>();
        }

        public static AttributeViewModel GetViewModel(AttributeContainer container, string key)
        {
            AttributeViewModel viewModel = null;
            var tuple = new Tuple<AttributeContainer, string>(container, key);
            attributes.TryGetValue(tuple, out viewModel);
            if (null == viewModel)
            {
                viewModel = new AttributeViewModel(container, key);
                attributes[tuple] = viewModel;
            }
            return viewModel;
        }

        private readonly AttributeContainer container;
        private Attribute attribute;

        public string Key
        {
            get { return attribute.Key; }
            set
            {
                CommandHistory.LogCommand(new RenameAttributeCommand(this, value));
                attribute.Key = value;

                RaisePropertyChanged("Key");
            }
        }

        public string Value
        {
            get { return attribute.Value.ToString(); }
            set
            {
                if (!IsInherited)
                {
                    CommandHistory.LogCommand(new SetAttributeValueCommand(this, value));
                    attribute.Value = Attribute.TryParse(value);
                    RaisePropertyChanged("Value");
                }
            }
        }

        public bool CanInherit
        {
            get { return false; }
        }

        public bool IsLocal
        {
            get { return true; }
        }

        public bool IsInherited
        {
            get { return false; }
            set { }
        }

        private AttributeViewModel(AttributeContainer container, string key)
        {
            this.container = container;
            
            attribute = container.GetAttribute(key);
            if (null == attribute)
            {
                attribute = new Attribute(key, 0);
            }
        }

        public void AddAttribute()
        {
            if (null == attribute.Parent)
            {
                container.AddAttribute(attribute);
            }
        }

        public void RemoveAttribute()
        {
            if (attribute.Parent == container)
            {
                container.RemoveAttribute(attribute);
            }
        }
    }
}
