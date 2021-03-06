//-----------------------------------------------------------------------
// <copyright file="Component.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Values;
using Kinectitude.Editor.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Kinectitude.Editor.Models
{
    internal delegate void ComponentPropertyEventHandler(Component component, PluginProperty property);

    internal sealed class Component : GameModel<IComponentScope>, IPropertyScope
    {
        private readonly Plugin plugin;
        private readonly List<Property> properties;

        public event PropertyEventHandler InheritedPropertyAdded;
        public event PropertyEventHandler InheritedPropertyRemoved;
        public event PropertyEventHandler InheritedPropertyChanged;
        public event ComponentPropertyEventHandler LocalPropertyChanged;

        public Plugin Plugin
        {
            get { return plugin; }
        }

        public string DisplayName
        {
            get { return plugin.Header; }
        }

        public string Type
        {
            get { return null != Scope ? Scope.GetDefinedName(plugin) : plugin.ClassName; }
        }

        public string Provides
        {
            get { return plugin.Provides; }
        }

        public IEnumerable<string> Requires
        {
            get { return plugin.Requires; }
        }

        public bool HasOwnValues
        {
            get { return Properties.Any(x => x.HasOwnValue); }
        }

        [DependsOn("IsRoot")]
        public bool IsInherited
        {
            get { return !IsRoot; }
        }

        public bool IsRoot
        {
            get { return null != Scope ? !Scope.HasInheritedComponent(plugin) : true; }
        }

        public IEnumerable<Property> Properties
        {
            get { return properties; }
        }

        public Component(Plugin plugin)
        {
            if (plugin.Type != PluginType.Component)
            {
                throw new ArgumentException("Plugin is not a component");
            }

            this.plugin = plugin;
            
            properties = new List<Property>();
            foreach (PluginProperty property in plugin.Properties)
            {
                AddProperty(new Property(property));
            }

            AddDependency<ScopeChanged>("Type");
            AddDependency<ScopeChanged>("IsRoot");
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
            property.PropertyChanged += OnPropertyValueChanged;
            property.EffectiveValueChanged += OnPropertyEffectiveValueChanged;
            properties.Add(property);
        }

        private void OnPropertyEffectiveValueChanged(PluginProperty property)
        {
            Notify(new EffectiveValueChanged(Plugin, property));
        }

        protected override void OnScopeDetaching(IComponentScope scope)
        {
            scope.InheritedComponentAdded -= OnInheritedComponentAdded;
            scope.InheritedComponentRemoved -= OnInheritedComponentRemoved;
            scope.InheritedPropertyChanged -= OnInheritedPropertyChanged;
        }

        protected override void OnScopeAttaching(IComponentScope scope)
        {
            scope.InheritedComponentAdded += OnInheritedComponentAdded;
            scope.InheritedComponentRemoved += OnInheritedComponentRemoved;
            scope.InheritedPropertyChanged += OnInheritedPropertyChanged;
        }

        public bool DependsOn(Component requiredComponent)
        {
            return Requires.Contains(requiredComponent.Provides);
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

        private void OnInheritedComponentAdded(Plugin plugin)
        {
            if (this.plugin == plugin)
            {
                if (null != InheritedPropertyAdded)
                {
                    foreach (PluginProperty property in plugin.Properties)
                    {
                        InheritedPropertyAdded(property);
                    }
                }

                NotifyPropertyChanged("Scope");
            }
        }

        private void OnInheritedComponentRemoved(Plugin plugin)
        {
            if (this.plugin == plugin)
            {
                if (null != InheritedPropertyRemoved)
                {
                    foreach (PluginProperty property in plugin.Properties)
                    {
                        InheritedPropertyRemoved(property);
                    }
                }

                NotifyPropertyChanged("Scope");
            }
        }

        private void OnInheritedPropertyChanged(PluginProperty property)
        {
            if (null != InheritedPropertyChanged)
            {
                InheritedPropertyChanged(property);
            }
        }

        private void OnPropertyValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (null != LocalPropertyChanged && e.PropertyName == "Value")
            {
                AbstractProperty property = sender as AbstractProperty;
                LocalPropertyChanged(this, property.PluginProperty);
            }
        }

        public bool IsOfType(Type type)
        {
            return Plugin.CoreType == type;
        }

        public Component DeepCopy()
        {
            Component copy = new Component(this.Plugin);

            foreach (AbstractProperty property in this.Properties)
            {
                if (property.HasOwnValue)
                {
                    copy.SetProperty(property.Name, property.Value);
                }
            }

            return copy;
        }

        #region IPropertyScope implementation

        public bool HasInheritedProperty(PluginProperty property)
        {
            return null != Scope ? Scope.HasInheritedComponent(Plugin) : false;
        }

        public bool HasInheritedNonDefaultValue(PluginProperty property)
        {
            return null != Scope ? Scope.HasInheritedNonDefaultProperty(Plugin, property) : false;
        }

        public Value GetInheritedValue(PluginProperty property)
        {
            return null != Scope ? Scope.GetInheritedValue(Plugin, property) : property.DefaultValue;
        }

        #endregion
    }
}
