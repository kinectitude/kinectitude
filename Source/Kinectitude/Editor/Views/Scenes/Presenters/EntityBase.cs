using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Values;
using System;

namespace Kinectitude.Editor.Views.Scenes.Presenters
{
    internal class EntityBase : BaseModel
    {
        private readonly Entity entity;

        public Entity Entity
        {
            get { return entity; }
        }

        protected EntityBase(Entity entity)
        {
            this.entity = entity;
        }

        private Component GetComponent<TComponent>() where TComponent : Kinectitude.Core.Base.Component
        {
            return entity.GetComponentByType(typeof(TComponent));
        }

        private Property GetProperty<TComponent>(string propertyName) where TComponent : Kinectitude.Core.Base.Component
        {
            Property property = null;

            var component = GetComponent<TComponent>();
            if (null != component)
            {
                property = component.GetProperty(propertyName);
            }

            return property;
        }

        protected void SetValue<TComponent, TValue>(string propertyName, TValue value) where TComponent : Kinectitude.Core.Base.Component
        {
            var component = GetComponent<TComponent>();
            if (null != component)
            {
                Property property = component.GetProperty(propertyName);
                property.Value = new Value(value.ToString());   // TODO: Change the usage of setvalue to not require a TValue type here
            }

            NotifyPropertyChanged(propertyName);
        }

        protected double GetDoubleValue<TComponent>(string propertyName) where TComponent : Kinectitude.Core.Base.Component
        {
            double result = 0;

            var property = GetProperty<TComponent>(propertyName);
            if (null != property)
            {
                result = property.GetDoubleValue();
            }

            return result;
        }

        protected float GetFloatValue<TComponent>(string propertyName) where TComponent : Kinectitude.Core.Base.Component
        {
            float result = 0;

            var property = GetProperty<TComponent>(propertyName);
            if (null != property)
            {
                result = property.GetFloatValue();
            }

            return result;
        }

        protected int GetIntValue<TComponent>(string propertyName) where TComponent : Kinectitude.Core.Base.Component
        {
            int result = 0;

            var property = GetProperty<TComponent>(propertyName);
            if (null != property)
            {
                result = property.GetIntValue();
            }

            return result;
        }

        protected long GetLongValue<TComponent>(string propertyName) where TComponent : Kinectitude.Core.Base.Component
        {
            long result = 0;

            var property = GetProperty<TComponent>(propertyName);
            if (null != property)
            {
                result = property.GetLongValue();
            }

            return result;
        }

        protected bool GetBoolValue<TComponent>(string propertyName) where TComponent : Kinectitude.Core.Base.Component
        {
            bool result = false;

            var property = GetProperty<TComponent>(propertyName);
            if (null != property)
            {
                result = property.GetBoolValue();
            }

            return result;
        }

        protected string GetStringValue<TComponent>(string propertyName) where TComponent : Kinectitude.Core.Base.Component
        {
            string result = "";

            var property = GetProperty<TComponent>(propertyName);
            if (null != property)
            {
                result = property.GetStringValue();
            }

            return result;
        }

        protected TEnum GetEnumValue<TComponent, TEnum>(string propertyName)
            where TComponent : Kinectitude.Core.Base.Component
            where TEnum : struct, IConvertible
        {
            TEnum result = default(TEnum);

            var property = GetProperty<TComponent>(propertyName);
            if (null != property)
            {
                result = property.GetEnumValue<TEnum>();
            }

            return result;
        }
    }
}
