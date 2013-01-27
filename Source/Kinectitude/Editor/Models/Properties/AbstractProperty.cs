using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Values;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Kinectitude.Editor.Models.Properties
{
    internal abstract class AbstractProperty : GameModel<IPropertyScope>, INameValue
    {
        private readonly AbstractProperty sourceProperty;

        public abstract PluginProperty PluginProperty { get; }

        public string Name
        {
            get { return PluginProperty.Name; }
        }

        public bool IsReadOnly
        {
            get { return null != sourceProperty; }
        }

        public bool IsEditable
        {
            get { return !IsReadOnly; }
        }

        public abstract Value Value { get; set; }

        public abstract bool HasOwnValue { get; }

        public abstract IEnumerable<object> AvailableValues { get; }

        public abstract ICommand ClearValueCommand { get; protected set; }

        protected AbstractProperty(AbstractProperty sourceProperty = null)
        {
            this.sourceProperty = sourceProperty;

            if (null != sourceProperty)
            {
                sourceProperty.PropertyChanged += OnSourcePropertyChanged;
            }

            AddDependency<ScopeChanged>("Value");
        }

        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        protected override void OnScopeDetaching(IPropertyScope scope)
        {
            scope.InheritedPropertyAdded -= OnInheritedPropertyAdded;
            scope.InheritedPropertyRemoved -= OnInheritedPropertyRemoved;
            scope.InheritedPropertyChanged -= OnInheritedPropertyChanged;
        }

        protected override void OnScopeAttaching(IPropertyScope scope)
        {
            scope.InheritedPropertyAdded += OnInheritedPropertyAdded;
            scope.InheritedPropertyRemoved += OnInheritedPropertyRemoved;
            scope.InheritedPropertyChanged += OnInheritedPropertyChanged;
        }

        private void OnInheritedPropertyAdded(PluginProperty property)
        {
            if (property.Name == Name)
            {
                NotifyPropertyChanged("Value");
            }
        }

        private void OnInheritedPropertyRemoved(PluginProperty property)
        {
            if (property.Name == Name)
            {
                NotifyPropertyChanged("Value");
            }
        }

        private void OnInheritedPropertyChanged(PluginProperty property)
        {
            if (property.Name == Name)
            {
                NotifyPropertyChanged("Value");
            }
        }

        public Property DeepCopy()
        {
            var property = new Property(PluginProperty);

            if (HasOwnValue)
            {
                property.Value = Value;
            }

            return property;
        }

        public Value GetInheritedValue()
        {
            return null != Scope ? Scope.GetInheritedValue(PluginProperty) : PluginProperty.DefaultValue;
        }

        public double GetDoubleValue()
        {
            return Value.GetDoubleValue();
        }

        public float GetFloatValue()
        {
            return Value.GetFloatValue();
        }

        public int GetIntValue()
        {
            return Value.GetIntValue();
        }

        public long GetLongValue()
        {
            return Value.GetLongValue();
        }

        public bool GetBoolValue()
        {
            return Value.GetBoolValue();
        }

        public string GetStringValue()
        {
            return Value.GetStringValue();
        }

        public T GetEnumValue<T>() where T : struct, IConvertible
        {
            return Value.GetEnumValue<T>();
        }
    }
}
