//-----------------------------------------------------------------------
// <copyright file="Manager.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Values;
using Kinectitude.Editor.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kinectitude.Editor.Models
{
    internal sealed class Manager : GameModel<IManagerScope>, IPropertyScope
    {
        private readonly Plugin plugin;
        private readonly List<Property> properties;

        public string Type
        {
            get { return null != Scope ? Scope.GetDefinedName(plugin) : plugin.ClassName; }
        }

        public Plugin Plugin
        {
            get { return plugin; }
        }

        public string DisplayName
        {
            get { return plugin.Header; }
        }

        public bool IsRequired
        {
            get { return null != Scope ? Scope.RequiresManager(this) : false; }
        }

        public IEnumerable<Property> Properties
        {
            get { return properties; }
        }

        public Manager(Plugin plugin)
        {
            if (plugin.Type != PluginType.Manager)
            {
                throw new ArgumentException("Plugin is not a manager");
            }

            this.plugin = plugin;

            properties = new List<Property>();
            foreach (PluginProperty property in plugin.Properties)
            {
                AddProperty(new Property(property));
            }

            AddDependency<ScopeChanged>("Type");
            AddDependency<ScopeChanged>("IsRequired");
            AddDependency<DefineAdded>("Type", e => e.Define.Class == Plugin.ClassName);
            AddDependency<DefinedNameChanged>("Type", e => e.Plugin == Plugin);
            AddDependency<DefineRemoved>("Type", e => e.Define.Class == Plugin.ClassName);
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }

        private void AddProperty(Property property)
        {
            property.Scope = this;
            property.EffectiveValueChanged += OnPropertyEffectiveValueChanged;
            properties.Add(property);
        }

        private void OnPropertyEffectiveValueChanged(PluginProperty property)
        {
            Notify(new EffectiveValueChanged(Plugin, property));
        }

        public Property GetProperty(string name)
        {
            return Properties.FirstOrDefault(x => x.Name == name);
        }

        public void SetProperty(string name, Value value)
        {
            Property property = GetProperty(name);
            if (null != property)
            {
                property.Value = value;
            }
        }

        #region IPropertyScope implementation

        public event PropertyEventHandler InheritedPropertyAdded { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyRemoved { add { } remove { } }
        public event PropertyEventHandler InheritedPropertyChanged { add { } remove { } }

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
    }
}
