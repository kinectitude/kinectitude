using System.ComponentModel;
using EditorModels.Models;
using Attribute = EditorModels.Models.Attribute;
using EditorModels.ViewModels.Interfaces;

namespace EditorModels.ViewModels
{
    internal delegate void KeyChangedEventHandler(string oldKey, string newKey);
    
    internal sealed class AttributeViewModel : BaseViewModel
    {
        private const string DefaultValue = "";

        private readonly Attribute attribute;
        private IAttributeContainer container;
        private IAttributeScope scope;
        private bool inherited;

#if TEST

        public Attribute Attribute
        {
            get { return attribute; }
        }

#endif

        public event KeyChangedEventHandler KeyChanged;

        public string Key
        {
            get { return attribute.Key; }
            set
            {
                if (IsLocal && attribute.Key != value && !KeyExists(value))
                {
                    string oldKey = attribute.Key;
                    attribute.Key = value;

                    NotifyKeyChanged(oldKey, attribute.Key);
                }
            }
        }

        public bool IsInherited
        {
            get { return inherited; }
            set
            {
                if (inherited != value)
                {
                    inherited = value;

                    if (!inherited && null != container)
                    {
                        container.AddAttribute(attribute);
                    }
                    else if (null != container)
                    {
                        container.RemoveAttribute(attribute);
                    }

                    NotifyPropertyChanged("IsInherited", "IsLocal", "Key", "Value");
                }
            }
        }

        public bool IsLocal
        {
            get { return !IsInherited; }
        }

        public bool CanInherit
        {
            get { return null != scope ? scope.HasInheritedAttribute(Key) : false; }
        }

        public string Value
        {
            get
            {
                if (IsLocal)
                {
                    return attribute.Value;
                }

                return null != scope ? scope.GetInheritedValue(Key) : DefaultValue;
            }
            set
            {
                if (IsLocal)
                {
                    if (attribute.Value != value)
                    {
                        attribute.Value = value;
                        NotifyPropertyChanged("Value");
                    }
                }
            }
        }

        public AttributeViewModel(string key)
        {
            attribute = new Attribute();

            Key = key;
            Value = DefaultValue;
        }

        public void SetScope(IAttributeContainer container, IAttributeScope scope)
        {
            if (null != this.scope)
            {
                this.scope.InheritedAttributeAdded -= OnInheritedAttributeChanged;
                this.scope.InheritedAttributeRemoved -= OnInheritedAttributeChanged;
                this.scope.InheritedAttributeChanged -= OnInheritedAttributeChanged;
            }

            if (null != this.container)
            {
                if (IsLocal)
                {
                    this.container.RemoveAttribute(attribute);
                }
            }

            this.scope = scope;
            this.container = container;

            if (null != this.scope)
            {
                this.scope.InheritedAttributeAdded += OnInheritedAttributeChanged;
                this.scope.InheritedAttributeRemoved += OnInheritedAttributeChanged;
                this.scope.InheritedAttributeChanged += OnInheritedAttributeChanged;
            }

            if (null != this.container)
            {
                if (IsLocal)
                {
                    this.container.AddAttribute(attribute);
                }
            }
        }

        private bool KeyExists(string key)
        {
            if (null == scope)
            {
                return false;
            }

            return scope.HasInheritedAttribute(key) || scope.HasLocalAttribute(key);
        }

        private void OnInheritedAttributeChanged(string key)
        {
            if (key == Key)
            {
                NotifyPropertyChanged("CanInherit");
                if (IsInherited)
                {
                    NotifyPropertyChanged("Value");
                }
            }
        }

        private void NotifyKeyChanged(string oldKey, string newKey)
        {
            if (null != KeyChanged)
            {
                KeyChanged(oldKey, newKey);
            }
            NotifyPropertyChanged("Key");
        }
    }
}
