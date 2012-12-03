using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models
{
    internal delegate void KeyChangedEventHandler(string oldKey, string newKey);
    
    internal sealed class Attribute : GameModel<IAttributeScope>
    {
        private const string DefaultValue = "";

        private string key;
        private string value;
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
                    
                    Workspace.Instance.CommandHistory.Log(
                        "rename attribute to '" + value + "'",
                        () => Key = value,
                        () => Key = oldKey
                    );

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
                    bool oldInherited = inherited;

                    Workspace.Instance.CommandHistory.Log(
                        "toggle attribute inheritance",
                        () => IsInherited = value,
                        () => IsInherited = oldInherited
                    );

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

        public bool CanInherit
        {
            get { return null != Scope ? Scope.HasInheritedAttribute(Key) : false; }
        }

        [DependsOn("IsLocal")]
        public string Value
        {
            get
            {
                if (IsLocal)
                {
                    return value;
                }

                return null != Scope ? Scope.GetInheritedValue(Key) : DefaultValue;
            }
            set
            {
                if (IsLocal && this.value != value)
                {
                    string oldValue = this.value;

                    Workspace.Instance.CommandHistory.Log(
                        "change attribute value",
                        () => Value = value,
                        () => Value = oldValue
                    );

                    this.value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }

        public Attribute(string key)
        {
            this.key = key;
            this.value = DefaultValue;

            AddDependency<ScopeChanged>("CanInherit");
            AddDependency<ScopeChanged>("Value");
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void OnScopeDetaching(IAttributeScope scope)
        {
            scope.InheritedAttributeAdded -= OnInheritedAttributeChanged;
            scope.InheritedAttributeRemoved -= OnInheritedAttributeChanged;
            scope.InheritedAttributeChanged -= OnInheritedAttributeChanged;
        }

        protected override void OnScopeAttaching(IAttributeScope scope)
        {
            scope.InheritedAttributeAdded += OnInheritedAttributeChanged;
            scope.InheritedAttributeRemoved += OnInheritedAttributeChanged;
            scope.InheritedAttributeChanged += OnInheritedAttributeChanged;
        }

        private bool KeyExists(string key)
        {
            if (null == Scope)
            {
                return false;
            }

            return Scope.HasInheritedAttribute(key) || Scope.HasLocalAttribute(key);
        }

        private void OnInheritedAttributeChanged(string key)
        {
            if (key == Key)
            {
                NotifyPropertyChanged("Scope");
            }
        }

        public Attribute DeepCopy()
        {
            return new Attribute(this.Key) { Value = this.Value };
        }
    }
}
