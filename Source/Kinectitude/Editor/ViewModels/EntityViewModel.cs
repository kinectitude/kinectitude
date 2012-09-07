using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.Base;
using Kinectitude.Core.Components;
using Kinectitude.Render;

namespace Kinectitude.Editor.ViewModels
{
    internal class EntityViewModel : BaseModel
    {
        public static EntityViewModel Create(Entity entity, Component component)
        {
            if (component.IsOfType(typeof(RenderComponent)))
            {
                return new BasicEntityViewModel(entity);
            }
            else if (component.IsOfType(typeof(TextRenderComponent)))
            {
                return new TextEntityViewModel(entity);
            }
            else if (component.IsOfType(typeof(ImageRenderComponent)))
            {
                return new SpriteEntityViewModel(entity);
            }

            return new EntityViewModel(entity);
        }

        private readonly Entity entity;

        public double X
        {
            get { return GetValue<double>(typeof(TransformComponent), "X") - Width / 2.0d; }
            set
            {
                SetValue<double>(typeof(TransformComponent), "X", value + Width / 2.0d);
                NotifyPropertyChanged("X");
            }
        }

        public double Y
        {
            get { return GetValue<double>(typeof(TransformComponent), "Y") - Height / 2.0d; }
            set
            {
                SetValue<double>(typeof(TransformComponent), "Y", value + Height / 2.0d);
                NotifyPropertyChanged("Y");
            }
        }

        public double Width
        {
            get { return GetValue<double>(typeof(TransformComponent), "Width"); }
            set
            {
                SetValue<double>(typeof(TransformComponent), "Width", value);
                NotifyPropertyChanged("Width");
            }
        }

        public double Height
        {
            get { return GetValue<double>(typeof(TransformComponent), "Height"); }
            set
            {
                SetValue<double>(typeof(TransformComponent), "Height", value);
                NotifyPropertyChanged("Height");
            }
        }

        public float Opacity
        {
            get { return GetValue<float>(typeof(BaseRenderComponent), "Opacity"); }
            set
            {
                SetValue(typeof(BaseRenderComponent), "Opacity", value);
                NotifyPropertyChanged("Opacity");
            }
        }

        public EntityViewModel(Entity entity)
        {
            this.entity = entity;
        }

        protected void SetValue<T>(Type componentType, string propertyName, T value)
        {
            Component component = entity.GetComponentByType(componentType);
            if (null != component)
            {
                Property property = component.GetProperty(propertyName);
                property.Value = value;
            }
        }

        protected T GetValue<T>(Type componentType, string propertyName)
        {
            Component component = entity.GetComponentByType(componentType);
            if (null != component)
            {
                Property property = component.GetProperty(propertyName);
                if (null != property)
                {
                    return property.GetValue<T>();
                }
            }
            
            return default(T);
        }
    }
}
