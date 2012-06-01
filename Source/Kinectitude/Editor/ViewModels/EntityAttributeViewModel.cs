using System.ComponentModel;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Commands.Attribute;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.Models.Base;
using Attribute = Kinectitude.Editor.Models.Base.Attribute;

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class EntityAttributeViewModel : BaseModel, IAttributeViewModel
    {
        private readonly Entity entity;
        private EntityAttributeViewModel inheritedViewModel;
        private Attribute attribute;

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
                    CommandHistory.LogCommand(new RenameAttributeCommand(this, value));

                    string oldKey = attribute.Key;
                    attribute.Key = value;
                    FindInheritedViewModel();

                    RaisePropertyChanged("Key");

                    EntityViewModel entityViewModel = EntityViewModel.GetViewModel(entity);
                    entityViewModel.RaiseAttributeAvailable(oldKey, value);
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
                    CommandHistory.LogCommand(new SetAttributeValueCommand(this, value));
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
            get { return null != inheritedViewModel && attribute == inheritedViewModel.Attribute; }
            set
            {
                CommandHistory.LogCommand(new SetAttributeInheritedCommand(this, value));
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

                    RaisePropertyChanged("Key");
                    RaisePropertyChanged("Value");
                    RaisePropertyChanged("IsInherited");
                    RaisePropertyChanged("IsLocal");
                }
            }
        }

        public EntityAttributeViewModel(Entity entity, string key)
        {
            this.entity = entity;
            attribute = entity.GetAttribute(key);

            foreach (Entity prototype in entity.Prototypes)
            {
                EntityViewModel prototypeViewModel = EntityViewModel.GetViewModel(prototype);
                EntityAttributeViewModel attributeViewModel = prototypeViewModel.GetEntityAttributeViewModel(key);

                if (null != attributeViewModel)
                {
                    inheritedViewModel = attributeViewModel;
                    inheritedViewModel.PropertyChanged += OnPropertyChanged;
                }
            }

            if (null == attribute)
            {
                if (null != inheritedViewModel)
                {
                    // Attribute exists and is inherited
                    attribute = inheritedViewModel.Attribute;
                }
                else
                {
                    // Attribute does not exist
                    attribute = new Attribute(key, 0);
                }
            }
        }

        public void FindInheritedViewModel()
        {
            if (null != inheritedViewModel)
            {
                inheritedViewModel.PropertyChanged -= OnPropertyChanged;
            }

            inheritedViewModel = null;

            foreach (Entity prototype in entity.Prototypes)
            {
                EntityViewModel prototypeViewModel = EntityViewModel.GetViewModel(prototype);
                EntityAttributeViewModel attributeViewModel = prototypeViewModel.GetEntityAttributeViewModel(Key);

                if (null != attributeViewModel)
                {
                    inheritedViewModel = attributeViewModel;
                    inheritedViewModel.PropertyChanged += OnPropertyChanged;

                    if (IsInherited)
                    {
                        attribute = inheritedViewModel.Attribute;
                    }

                    break;
                }
            }

            RaisePropertyChanged("CanInherit");
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            EntityAttributeViewModel sourceAttribute = sender as EntityAttributeViewModel;
            if (null != sourceAttribute)
            {
                if (IsInherited)
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
                        FindInheritedViewModel();
                        RaisePropertyChanged("IsInherited");
                        //RaisePropertyChanged("CanInherit");
                        //RaisePropertyChanged("Key");
                        //RaisePropertyChanged("Value");
                    }
                }
                else
                {
                    FindInheritedViewModel();
                }
            }
        }

        public void AddAttribute()
        {
            if (IsLocal)
            {
                entity.AddAttribute(attribute);
            }
        }

        public void RemoveAttribute()
        {
            if (IsLocal)
            {
                entity.RemoveAttribute(attribute);
            }
        }

        /*public void OnAttributeAvailable(object sender, EventArgs<string> args)
        {
            if (args.Value == Key)
            {
                FindInheritedViewModel();
            }
        }*/
    }
}
