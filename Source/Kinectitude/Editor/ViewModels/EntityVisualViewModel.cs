﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.Base;
using Kinectitude.Core.Components;
using Kinectitude.Render;

namespace Kinectitude.Editor.ViewModels
{
    internal class EntityVisualViewModel : BaseModel
    {
        public static EntityVisualViewModel Create(Entity entity)
        {
            Component component = entity.GetComponentByRole(typeof(IRender).FullName);
            
            if (null != component)
            {
                if (component.IsOfType(typeof(RenderComponent)))
                {
                    return new BasicEntityVisualViewModel(entity);
                }
                else if (component.IsOfType(typeof(TextRenderComponent)))
                {
                    return new TextEntityVisualViewModel(entity);
                }
                else if (component.IsOfType(typeof(ImageRenderComponent)))
                {
                    return new SpriteEntityVisualViewModel(entity);
                }
            }

            return new EntityVisualViewModel(entity);
        }

        private readonly Entity entity;

        public Entity Model
        {
            get { return entity; }
        }

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

        public EntityVisualViewModel(Entity entity)
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
