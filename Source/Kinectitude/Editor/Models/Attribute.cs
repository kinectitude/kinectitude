using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Storage;
using System.Windows.Input;

namespace Kinectitude.Editor.Models
{
    internal delegate void NameChangedEventHandler(string oldName, string newName);
    
    internal sealed class Attribute : GameModel<IAttributeScope>
    {
        public const string DefaultValue = "";

        private string name;
        private object val;
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

        public bool IsInherited
        {
            get { return inherited; }
            set
            {
                if (inherited != value)
                {
                    //bool oldInherited = inherited;

                    //Workspace.Instance.CommandHistory.Log(
                    //    "toggle attribute inheritance",
                    //    () => IsInherited = value,
                    //    () => IsInherited = oldInherited
                    //);

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

        public bool IsEditable
        {
            get { return true; }
        }

        //public bool CanInherit
        //{
        //    get { return null != Scope ? Scope.HasInheritedAttribute(Key) : false; }
        //}

        [DependsOn("IsLocal")]
        public object Value
        {
            get
            {
                //if (IsLocal)
                //{
                //    return value;
                //}

                //return null != Scope ? Scope.GetInheritedValue(Key) : DefaultValue;

                return val ?? GetInheritedValue();
            }
            set
            {
                //if (IsLocal && this.value != value)
                if (val != value)
                {
                    object oldValue = val;

                    Workspace.Instance.CommandHistory.Log(
                        "change attribute value",
                        () => Value = value,
                        () => Value = oldValue
                    );

                    val = value;
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

            //AddDependency<ScopeChanged>("CanInherit");
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
            return new Attribute(this.Name) { Value = this.Value };
        }

        private object GetInheritedValue()
        {
            return null != Scope ? Scope.GetInheritedValue(Name) : DefaultValue;
        }
    }
}
