using System.Collections.Generic;
using Kinectitude.Editor.Base;
using Attribute = Kinectitude.Editor.Models.Base.Attribute;
using System.ComponentModel;
using Kinectitude.Editor.Models.Base;
using System.Collections.Specialized;
using Kinectitude.Editor.Commands.Attribute;

namespace Kinectitude.Editor.ViewModels
{
    public class AttributeViewModel : BaseModel
    {
        private static readonly Dictionary<Entity, Dictionary<string, AttributeViewModel>> attributesForEntities;
        private static readonly Dictionary<Attribute, AttributeViewModel> attributes;

        static AttributeViewModel()
        {
            attributesForEntities = new Dictionary<Entity, Dictionary<string, AttributeViewModel>>();
            attributes = new Dictionary<Attribute, AttributeViewModel>();
        }

        public static AttributeViewModel GetViewModel(Entity entity, string key)
        {
            Dictionary<string, AttributeViewModel> attributesForEntity = null;
            attributesForEntities.TryGetValue(entity, out attributesForEntity);
            if (null == attributesForEntity)
            {
                attributesForEntity = new Dictionary<string,AttributeViewModel>();
                attributesForEntities[entity] = attributesForEntity;
            }

            AttributeViewModel viewModel = null;
            attributesForEntity.TryGetValue(key, out viewModel);
            if (null == viewModel)
            {
                viewModel = new AttributeViewModel(entity, key);
                attributesForEntity[key] = viewModel;
            }

            return viewModel;
        }

        public static AttributeViewModel GetViewModel(Attribute attribute)
        {
            AttributeViewModel viewModel = null;
            attributes.TryGetValue(attribute, out viewModel);
            if (null == viewModel)
            {
                viewModel = new AttributeViewModel(attribute);
                attributes[attribute] = viewModel;
            }
            return viewModel;
        }

        private readonly Entity entity;
        private Attribute attribute;
        private AttributeViewModel inheritedViewModel;

        public Attribute Attribute
        {
            get { return attribute; }
            set { attribute = value; }
        }

        public Entity Entity
        {
            get { return entity; }
        }

        public string Key
        {
            get { return attribute.Key; }
            set
            {
                if (!IsInherited)
                {
                    RenameAttributeCommand command = new RenameAttributeCommand(this, value);
                    command.Execute();
                }
            }
        }

        public string Value
        {
            get { return attribute.Value.ToString(); }
            set
            {
                if (!IsInherited)
                {
                    SetAttributeValueCommand command = new SetAttributeValueCommand(this, value);
                    command.Execute();
                }
            }
        }

        public AttributeViewModel InheritedAttribute
        {
            get { return inheritedViewModel; }
            set
            {
                if (null != inheritedViewModel)
                {
                    inheritedViewModel.PropertyChanged -= OnPropertyChanged;
                }

                inheritedViewModel = value;
                
                if (null != inheritedViewModel)
                {
                    inheritedViewModel.PropertyChanged += OnPropertyChanged;
                }
                RaisePropertyChanged("CanInherit");
            }
        }

        public bool CanInherit
        {
            get { return null != inheritedViewModel; }
        }

        public bool IsLocal
        {
            get { return !IsInherited; }
        }

        public bool IsInherited
        {
            get { return CanInherit && attribute == InheritedAttribute.Attribute; }
            set
            {
                SetAttributeInheritedCommand command = new SetAttributeInheritedCommand(this, value);
                command.Execute();
            }
        }

        private AttributeViewModel(Entity entity, string key)
        {
            this.entity = entity;
            attribute = entity.GetAttribute(key);
            FindInheritedAttribute(key);
            if (null == attribute)
            {
                attribute = InheritedAttribute.Attribute;
            }
        }

        private AttributeViewModel(Attribute attribute)
        {
            this.attribute = attribute;
        }

        public void FindInheritedAttribute(string key)
        {
            foreach (Entity prototype in entity.Prototypes)
            {
                EntityViewModel prototypeViewModel = EntityViewModel.GetViewModel(prototype);

                foreach (AttributeViewModel viewModel in prototypeViewModel.Attributes)
                {
                    if (viewModel.Key == key)
                    {
                        InheritedAttribute = viewModel;
                        return;
                    }
                }
            }
            InheritedAttribute = null;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            AttributeViewModel sourceAttribute = sender as AttributeViewModel;
            if (null != sourceAttribute)
            {
                if (args.PropertyName == "Key")
                {
                    RaisePropertyChanged("Key");
                }
                else if (args.PropertyName == "Value")
                {
                    RaisePropertyChanged("Value");
                }
            }
        }
    }
}
