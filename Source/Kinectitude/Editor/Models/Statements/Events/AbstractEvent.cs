//-----------------------------------------------------------------------
// <copyright file="AbstractEvent.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Statements.Base;
using Kinectitude.Editor.Models.Values;
using System.Collections.Generic;
using System.Linq;

namespace Kinectitude.Editor.Models.Statements.Events
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

        public void SetProperty(string name, Value value)
        {
            AbstractProperty property = GetProperty(name);
            if (null != property)
            {
                property.Value = value;
            }
        }

        #region IPropertyScope implementation

        public bool HasInheritedProperty(PluginProperty property)
        {
            return false;
        }

        public bool HasInheritedNonDefaultValue(PluginProperty property)
        {
            return false;
        }

        public Value GetInheritedValue(PluginProperty property)
        {
            return property.DefaultValue;
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

        public sealed override AbstractStatement CreateReadOnly()
        {
            return new ReadOnlyEvent(this);
        }
    }
}
