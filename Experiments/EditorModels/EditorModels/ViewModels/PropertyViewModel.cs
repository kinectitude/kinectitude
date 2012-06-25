using System.ComponentModel;
using EditorModels.Models;
using Component = EditorModels.Models.Component;

namespace EditorModels.ViewModels
{
    internal sealed class PropertyViewModel : BaseViewModel
    {
        private bool inherited;
        private Component component;
        private PropertyViewModel inheritedProperty;
        private readonly Property property;

        public string Name
        {
            get { return property.Name; }
            set
            {
                if (property.Name != value)
                {
                    property.Name = value;
                    NotifyPropertyChanged("Name");
                }
            }
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
                if (inherited != value)
                {
                    inherited = value;

                    if (!inherited && null != component)
                    {
                        component.AddProperty(property);
                    }
                    else if (null != component)
                    {
                        component.RemoveProperty(property);
                    }

                    NotifyPropertyChanged("IsLocal");
                    NotifyPropertyChanged("IsInherited");
                    NotifyPropertyChanged("Key");
                    NotifyPropertyChanged("Value");
                }
            }
        }

        public bool CanInherit
        {
            get { return null != inheritedProperty; }
        }

        public object Value
        {
            get
            {
                object ret = null;

                if (IsLocal)
                {
                    ret = property.Value;
                }
                else if (IsInherited)
                {
                    ret = inheritedProperty.Value;
                }

                return ret;
            }
            set
            {
                if (property.Value != value)
                {
                    if (IsLocal)
                    {
                        property.Value = value;  // TODO: Serialize anything
                        NotifyPropertyChanged("Value");
                    }
                }
            }
        }

        public PropertyViewModel(string name)
        {
            property = new Property();

            Name = name;
            IsInherited = true;
        }

        public void SetComponent(Component component)
        {
            if (null != this.component)
            {
                if (IsLocal)
                {
                    this.component.RemoveProperty(property);
                }
            }

            this.component = component;

            if (null != this.component)
            {
                if (IsLocal)
                {
                    this.component.AddProperty(property);
                }
            }
        }

        public void SetInheritedProperty(PropertyViewModel property)
        {
            if (null != inheritedProperty)
            {
                inheritedProperty.PropertyChanged -= OnPropertyChanged;
            }

            inheritedProperty = property;

            if (null != inheritedProperty)
            {
                inheritedProperty.PropertyChanged += OnPropertyChanged;
            }

            NotifyPropertyChanged("CanInherit");
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (IsInherited && args.PropertyName == "Value")
            {
                NotifyPropertyChanged("Value");
            }
        }
    }
}
