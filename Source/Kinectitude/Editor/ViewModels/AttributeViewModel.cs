using System.Collections.Generic;
using Kinectitude.Editor.Base;
using Attribute = Kinectitude.Editor.Models.Base.Attribute;
using System.ComponentModel;
using Kinectitude.Editor.Models.Base;
using System.Collections.Specialized;

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
        private bool canInherit;
        private bool inherited;

        public Attribute Attribute
        {
            get { return attribute; }
        }

        public string Key
        {
            get { return attribute.Key; }
            set
            {
                if (!inherited)
                {
                    attribute.Key = value; // TODO: Propagation and command
                    RaisePropertyChanged("Key");
                }
            }
        }

        public dynamic Value
        {
            get { return attribute.Value; }
            set
            {
                attribute.Value = value; // TODO: Propagation and command
                RaisePropertyChanged("Value");
            }
        }

        public bool CanInherit
        {
            get { return canInherit; }
        }

        public bool IsInherited
        {
            get { return inherited; }
            set
            {
                inherited = value;

                if (!inherited)
                {
                    attribute = new Attribute(inheritedViewModel.Key, inheritedViewModel.Value);
                    inheritedViewModel.PropertyChanged -= onPropertyChanged;
                    entity.AddAttribute(attribute);
                    inheritedViewModel = null;
                }
                else
                {
                    entity.RemoveAttribute(attribute);

                    foreach (Entity prototype in entity.Prototypes)
                    {
                        EntityViewModel prototypeViewModel = EntityViewModel.GetViewModel(prototype);

                        foreach (AttributeViewModel viewModel in prototypeViewModel.Attributes)
                        {
                            if (viewModel.Key == attribute.Key)
                            {
                                inheritedViewModel = viewModel;
                                inheritedViewModel.PropertyChanged += onPropertyChanged;
                                attribute = viewModel.Attribute;
                                inherited = true;
                            }
                        }
                    }
                }
                RaisePropertyChanged("IsInherited");
            }
        }

        private AttributeViewModel(Entity entity, string key)
        {
            this.entity = entity;

            attribute = entity.GetAttribute(key);
            inherited = false;

            if (null == attribute)
            {
                foreach (Entity prototype in entity.Prototypes)
                {
                    EntityViewModel prototypeViewModel = EntityViewModel.GetViewModel(prototype);

                    foreach (AttributeViewModel viewModel in prototypeViewModel.Attributes)
                    {
                        if (viewModel.Key == key)
                        {
                            inheritedViewModel = viewModel;
                            inheritedViewModel.PropertyChanged += onPropertyChanged;
                            attribute = viewModel.Attribute;
                            inherited = true;
                        }
                    }
                }

                if (null == inheritedViewModel)
                {

                }
            }
        }

        private AttributeViewModel(Attribute attribute)
        {
            this.attribute = attribute;
            this.inherited = false;
        }

        private void onAttributesChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (AttributeViewModel viewModel in args.NewItems)
                {

                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {

            }
        }

        private void onPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            AttributeViewModel sourceAttribute = sender as AttributeViewModel;
            if (null != sourceAttribute)
            {
                if (args.PropertyName == "Key")
                {
                    Key = sourceAttribute.Key;
                }
                else if (args.PropertyName == "Value")
                {
                    Value = sourceAttribute.Value;
                }
            }
        }
    }
}
