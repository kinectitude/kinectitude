
namespace Kinectitude.Editor.Models
{
    internal sealed class Property : AbstractProperty
    {
        private readonly string name;
        private object value;
        private bool inherited;

        public override string Name
        {
            get { return name; }
        }

        public override bool IsInherited
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
        public override bool IsLocal
        {
            get { return !IsInherited; }
        }

        [DependsOn("Scope")]
        public override bool CanInherit
        {
            get { return null != scope ? scope.HasInheritedProperty(Name) : false; }
        }

        [DependsOn("CanInherit")]
        public override bool IsRoot
        {
            get { return !CanInherit; }
        }

        [DependsOn("IsInherited")]
        [DependsOn("Scope")]
        public override object Value
        {
            get
            {
                if (IsInherited && !IsRoot)
                {
                    return null != scope ? scope.GetInheritedValue(Name) : 0;   // TODO: Get actual default
                }

                return value;
            }
            set
            {
                if (this.value != value)
                {
                    if (IsRoot || IsLocal)
                    {
                        this.value = value;  // TODO: Serialize anything
                        IsInherited = false;
                        NotifyPropertyChanged("Value");
                    }
                }
            }
        }

        public Property(string name)
        {
            this.name = name;
            this.inherited = true;
        }

        public T GetValue<T>()
        {
            T ret = default(T);

            if (null != Value)
            {
                ret = (T)System.Convert.ChangeType(Value, typeof(T));
            }

            return ret;
        }
    }
}
