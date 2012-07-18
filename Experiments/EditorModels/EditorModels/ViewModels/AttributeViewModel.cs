using System.ComponentModel;
using EditorModels.ViewModels.Interfaces;

namespace EditorModels.ViewModels
{
    internal delegate void KeyChangedEventHandler(string oldKey, string newKey);
    
    internal sealed class AttributeViewModel : BaseViewModel
    {
        private const string DefaultValue = "";

        private string key;
        private string value;
        private IAttributeScope scope;
        private bool inherited;

        public event KeyChangedEventHandler KeyChanged;

        public string Key
        {
            get { return key; }
            set
            {
                if (IsLocal && key != value && !KeyExists(value))
                {
                    string oldKey = key;
                    key = value;

                    if (null != KeyChanged)
                    {
                        KeyChanged(oldKey, key);
                    }

                    NotifyPropertyChanged("Key");
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
                    NotifyPropertyChanged("IsInherited");
                }
            }
        }

        [DependsOn("IsInherited")]
        public bool IsLocal
        {
            get { return !IsInherited; }
        }

        [DependsOn("Scope")]
        public bool CanInherit
        {
            get { return null != scope ? scope.HasInheritedAttribute(Key) : false; }
        }

        [DependsOn("IsLocal")]
        [DependsOn("Scope")]
        public string Value
        {
            get
            {
                if (IsLocal)
                {
                    return value;
                }

                return null != scope ? scope.GetInheritedValue(Key) : DefaultValue;
            }
            set
            {
                if (IsLocal)
                {
                    if (this.value != value)
                    {
                        this.value = value;
                        NotifyPropertyChanged("Value");
                    }
                }
            }
        }

        public AttributeViewModel(string key)
        {
            this.key = key;
            this.value = DefaultValue;
        }

        public void SetScope(IAttributeScope scope)
        {
            if (null != this.scope)
            {
                this.scope.InheritedAttributeAdded -= OnInheritedAttributeChanged;
                this.scope.InheritedAttributeRemoved -= OnInheritedAttributeChanged;
                this.scope.InheritedAttributeChanged -= OnInheritedAttributeChanged;
            }

            this.scope = scope;

            if (null != this.scope)
            {
                this.scope.InheritedAttributeAdded += OnInheritedAttributeChanged;
                this.scope.InheritedAttributeRemoved += OnInheritedAttributeChanged;
                this.scope.InheritedAttributeChanged += OnInheritedAttributeChanged;
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
                NotifyPropertyChanged("Scope");
            }
        }
    }
}
