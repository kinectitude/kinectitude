using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Values;
using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models
{
    internal sealed class Property : AbstractProperty
    {
        private readonly PluginProperty pluginProperty;
        //private object value;
        private bool inherited;

        public override PluginProperty PluginProperty
        {
            get { return pluginProperty; }
        }

        public override string Name
        {
            get { return PluginProperty.Name; }
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

        public override bool CanInherit
        {
            get { return null != Scope ? Scope.HasInheritedProperty(Name) : false; }
        }

        [DependsOn("CanInherit")]
        public override bool IsRoot
        {
            get { return !CanInherit; }
        }

        [DependsOn("IsInherited")]
        public override object Value
        {
            get
            {
                if (IsInherited && !IsRoot)
                {
                    return null != Scope ? Scope.GetInheritedValue(Name) : 0;   // TODO: Get actual default
                }

                return TypedValue.CurrentValue;
            }
            set
            {
                if (TypedValue.CurrentValue != value)
                {
                    if (IsRoot || IsLocal)
                    {
                        TypedValue.CurrentValue = value;  // TODO: Serialize anything
                        IsInherited = false;
                        NotifyPropertyChanged("Value");
                    }
                }
            }
        }

        public Value TypedValue { get; private set; }

        public Property(PluginProperty pluginProperty)
        {
            this.pluginProperty = pluginProperty;
            this.inherited = true;

            TypedValue = pluginProperty.CreateValue();

            AddDependency<ScopeChanged>("CanInherit");
            AddDependency<ScopeChanged>("Value");
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
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
