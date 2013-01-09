using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.Models.Properties;

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

        protected void SetValue<TComponent, TValue>(string propertyName, TValue value) where TComponent : Kinectitude.Core.Base.Component
        {
            Component component = entity.GetComponentByType(typeof(TComponent));
            if (null != component)
            {
                Property property = component.GetProperty(propertyName);
                property.Value = value;
            }

            NotifyPropertyChanged(propertyName);
        }

        protected TValue GetValue<TComponent, TValue>(string propertyName) where TComponent : Kinectitude.Core.Base.Component
        {
            Component component = entity.GetComponentByType(typeof(TComponent));
            if (null != component)
            {
                Property property = component.GetProperty(propertyName);
                if (null != property)
                {
                    return property.GetValue<TValue>();
                }
            }

            return default(TValue);
        }
    }
}
