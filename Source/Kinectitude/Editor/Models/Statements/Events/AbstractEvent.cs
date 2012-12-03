using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Statements;

namespace Kinectitude.Editor.Models
{
    internal abstract class AbstractEvent : CompositeStatement, IPropertyScope
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

        public override IEnumerable<Plugin> Plugins
        {
            get { return Statements.SelectMany(x => x.Plugins).Union(Enumerable.Repeat(Plugin, 1)).Distinct(); }
        }

        protected AbstractEvent(AbstractEvent inheritedEvent = null) : base(inheritedEvent)
        {
            properties = new List<AbstractProperty>();

            AddDependency<ScopeChanged>("Type");
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
            AbstractProperty property = GetProperty(name);
            if (null != property)
            {
                property.Value = value;
            }
        }

        #region IPropertyScope implementation

        public bool HasInheritedProperty(string name)
        {
            return false;
        }

        public object GetInheritedValue(string name)
        {
            return null;
        }

        #endregion

        public Event DeepCopy()
        {
            Event copy = new Event(Plugin);

            foreach (AbstractProperty property in Properties)
            {
                copy.SetProperty(property.Name, property.Value);
            }

            foreach (AbstractStatement statement in Statements)
            {
                copy.AddStatement(statement.DeepCopyStatement());
            }

            return copy;
        }

        public sealed override AbstractStatement DeepCopyStatement()
        {
            return DeepCopy();
        }

        public sealed override AbstractStatement CreateInheritor()
        {
            return new InheritedEvent(this);
        }
    }
}
