using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Statements.Base;
using System.Collections.Generic;
using System.Linq;

namespace Kinectitude.Editor.Models.Statements.Actions
{
    internal abstract class AbstractAction : AbstractStatement, IPropertyScope
    {
        private readonly List<AbstractProperty> properties;

        public event PropertyEventHandler InheritedPropertyAdded { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyRemoved { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyChanged { add { } remove { } }

        public abstract Plugin Plugin { get; }
        public abstract Header Header { get; }

        public string Type
        {
            get { return null != Scope ? Scope.GetDefinedName(Plugin) : Plugin.ClassName; }
        }

        public string Description
        {
            get { return Plugin.Description; }
        }

        public IEnumerable<AbstractProperty> Properties
        {
            get { return properties; }
        }

        public sealed override IEnumerable<Plugin> Plugins
        {
            get { return Enumerable.Repeat(Plugin, 1); }
        }

        protected AbstractAction(AbstractAction inheritedAction = null) : base(inheritedAction)
        {
            properties = new List<AbstractProperty>();
        }

        protected void AddProperty(AbstractProperty property)
        {
            property.Scope = this;
            properties.Add(property);
        }

        public AbstractProperty GetProperty(string name)
        {
            return Properties.FirstOrDefault(x => x.Name == name);
        }

        public void SetProperty(string name, object value)
        {
            if (IsLocal)
            {
                AbstractProperty property = GetProperty(name);
                if (null != property)
                {
                    property.Value = value;
                }
            }
        }

        #region IPropertyScope implementation

        public bool HasInheritedProperty(PluginProperty property)
        {
            return false;
        }

        public object GetInheritedValue(PluginProperty property)
        {
            return property.DefaultValue;
        }

        #endregion

        public sealed override AbstractStatement DeepCopyStatement()
        {
            Action copy = new Action(this.Plugin);

            foreach (AbstractProperty property in this.Properties)
            {
                copy.SetProperty(property.Name, property.Value);
            }

            return copy;
        }

        public sealed override AbstractStatement CreateInheritor()
        {
            return new ReadOnlyAction(this);
        }
    }
}
