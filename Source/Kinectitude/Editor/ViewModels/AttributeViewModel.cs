using System.Collections.Generic;
using Kinectitude.Editor.Base;
using Attribute = Kinectitude.Editor.Models.Base.Attribute;
using System.ComponentModel;
using Kinectitude.Editor.Models.Base;
using System.Collections.Specialized;
using Kinectitude.Editor.Commands.Attribute;
using Kinectitude.Editor.Commands.Base;

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
        private bool inherited;
        private Attribute attribute;
        private AttributeViewModel inheritedViewModel;

        public Attribute Attribute
        {
            get { return attribute; }
        }

        public string Key
        {
            get { return attribute.Key; }
            set
            {
                if (!IsInherited)
                {
                    CommandHistory.Instance.LogCommand(new RenameAttributeCommand(this, value));
                    attribute.Key = value;
                    FindInheritedAttribute(value);
                    RaisePropertyChanged("Key");
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
                    CommandHistory.Instance.LogCommand(new SetAttributeValueCommand(this, value));
                    attribute.Value = Attribute.TryParse(value);
                    RaisePropertyChanged("Value");
                }
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
            get { return inherited; }
            set
            {
                CommandHistory.Instance.LogCommand(new SetAttributeInheritedCommand(this, value));
                if (CanInherit)
                {
                    if (!value)
                    {
                        attribute = new Attribute(inheritedViewModel.Key, inheritedViewModel.Value);
                        entity.AddAttribute(attribute);
                    }
                    else
                    {
                        if (null != attribute)
                        {
                            entity.RemoveAttribute(attribute);
                        }
                        attribute = inheritedViewModel.Attribute;
                    }

                    inherited = value;

                    RaisePropertyChanged("Key");
                    RaisePropertyChanged("Value");
                    RaisePropertyChanged("IsInherited");
                    RaisePropertyChanged("IsLocal");
                }
            }
        }

        private AttributeViewModel(Entity entity, string key)
        {
            this.entity = entity;
            attribute = entity.GetAttribute(key);
            FindInheritedAttribute(key);
            if (null == attribute)
            {
                inherited = true;
                attribute = inheritedViewModel.Attribute;
            }
        }

        private AttributeViewModel(Attribute attribute)
        {
            this.attribute = attribute;
        }

        public void FindInheritedAttribute(string key)
        {
            if (null != inheritedViewModel)
            {
                inheritedViewModel.PropertyChanged -= OnPropertyChanged;
            }

            foreach (Entity prototype in entity.Prototypes)
            {
                EntityViewModel prototypeViewModel = EntityViewModel.GetViewModel(prototype);

                foreach (AttributeViewModel viewModel in prototypeViewModel.Attributes)
                {
                    if (viewModel.Key == key)
                    {
                        inheritedViewModel = viewModel;
                        inheritedViewModel.PropertyChanged += OnPropertyChanged;

                        if (IsInherited)
                        {
                            attribute = inheritedViewModel.Attribute;
                        }

                        RaisePropertyChanged("CanInherit");
                        return;
                    }
                }
            }

            RaisePropertyChanged("CanInherit");
            inheritedViewModel = null;
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
                else if (args.PropertyName == "IsInherited")
                {
                    FindInheritedAttribute(attribute.Key);
                    RaisePropertyChanged("IsInherited");
                    RaisePropertyChanged("CanInherit");
                    RaisePropertyChanged("Key");
                    RaisePropertyChanged("Value");
                }
            }
        }
    }
}
