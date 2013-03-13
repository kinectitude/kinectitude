using Kinectitude.Core.Data;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Values;
using Kinectitude.Editor.Storage;
using System.Windows.Input;

namespace Kinectitude.Editor.Models
{
    internal delegate void NameChangedEventHandler(string oldName, string newName);
    
    internal sealed class Attribute : GameModel<IAttributeScope>, IValueScope, INameValue
    {
        public static readonly Value DefaultValue = new Value(null, true);

        private string name;
        private Value val;
        private bool inherited;

        public event NameChangedEventHandler NameChanged;

        public string Name
        {
            get { return name; }
            set
            {
                if (IsLocal && name != value && !KeyExists(value))
                {
                    string oldKey = name;
                    
                    Workspace.Instance.CommandHistory.Log(
                        "rename attribute to '" + value + "'",
                        () => Name = value,
                        () => Name = oldKey
                    );

                    name = value;

                    if (null != NameChanged)
                    {
                        NameChanged(oldKey, name);
                    }

                    NotifyPropertyChanged("Key");
                }
            }
        }

        [DependsOn("Value")]
        public bool HasOwnValue
        {
            get { return null != val; }
        }

        public bool HasFileChooser
        {
            get { return false; }
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

        [DependsOn("Value")]
        public bool HasOwnOrInheritedValue
        {
            get { return HasOwnValue || InheritsNonDefaultValue(); }
        }

        [DependsOn("IsInherited")]
        public bool IsLocal
        {
            get { return !IsInherited; }
        }

        public bool IsEditable
        {
            get { return true; }
        }

        public Value Value
        {
            get
            {
                return val ?? GetInheritedValue();
            }
            set
            {
                if (val != value)
                {
                    var oldValue = val;

                    if (null != val)
                    {
                        val.Scope = null;
                    }

                    val = value;

                    if (null != val)
                    {
                        val.Scope = this;
                    }

                    Workspace.Instance.CommandHistory.Log(
                        "change attribute value",
                        () => Value = value,
                        () => Value = oldValue
                    );

                    NotifyPropertyChanged("Value");
                }
            }
        }

        public ICommand ClearValueCommand { get; private set; }

        public Attribute(string name)
        {
            this.name = name;
            this.inherited = false;

            ClearValueCommand = new DelegateCommand(parameter => HasOwnValue, parameter => Value = null);

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
            if (key == Name)
            {
                NotifyPropertyChanged("Value");
            }
        }

        public Attribute DeepCopy()
        {
            var copy = new Attribute(this.Name);
            
            if (HasOwnValue)
            {
                copy.Value = this.Value;
            };

            return copy;
        }

        private Value GetInheritedValue()
        {
            return null != Scope ? Scope.GetInheritedValue(Name) : DefaultValue;
        }

        private bool InheritsNonDefaultValue()
        {
            return null != Scope ? Scope.HasInheritedNonDefaultAttribute(Name) : false;
        }
    }
}
