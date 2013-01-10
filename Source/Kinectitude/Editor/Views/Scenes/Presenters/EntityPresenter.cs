using Kinectitude.Core.Components;
using Kinectitude.Editor.Models;
using Kinectitude.Render;
using System.Collections.Specialized;

namespace Kinectitude.Editor.Views.Scenes.Presenters
{
    internal class EntityPresenter : EntityBase
    {
        private EntityVisual visual;
        private Component transform;
        private Component render;
        
        [DependsOn("TransformComponent")]
        public double X
        {
            get { return GetValue<TransformComponent, double>("X") - Width / 2.0d; }
            set { SetValue<TransformComponent, double>("X", value + Width / 2.0d); }
        }

        [DependsOn("TransformComponent")]
        public double Y
        {
            get { return GetValue<TransformComponent, double>("Y") - Height / 2.0d; }
            set { SetValue<TransformComponent, double>("Y", value + Height / 2.0d); }
        }

        [DependsOn("TransformComponent")]
        public double Width
        {
            get { return GetValue<TransformComponent, double>("Width"); }
            set { SetValue<TransformComponent, double>("Width", value); }
        }

        [DependsOn("TransformComponent")]
        public double Height
        {
            get { return GetValue<TransformComponent, double>("Height"); }
            set { SetValue<TransformComponent, double>("Height", value); }
        }

        [DependsOn("TransformComponent")]
        public double Rotation
        {
            get { return GetValue<TransformComponent, double>("Rotation"); }
            set { SetValue<TransformComponent, double>("Rotation", value); }
        }

        [DependsOn("RenderComponent")]
        public float Opacity
        {
            get
            {
                if (null != RenderComponent)
                {
                    var property = render.GetProperty("Opacity");
                    if (null != property)
                    {
                        return property.GetValue<float>();
                    }
                }

                return 1.0f;
            }
            set
            {
                if (null != RenderComponent)
                {
                    render.SetProperty("Opacity", value);
                }
            }
        }

        public Component TransformComponent
        {
            get { return transform; }
            set
            {
                if (transform != value)
                {
                    if (null != transform)
                    {
                        transform.LocalPropertyChanged -= OnPropertyChanged;
                    }

                    transform = value;

                    if (null != transform)
                    {
                        transform.LocalPropertyChanged += OnPropertyChanged;
                    }

                    NotifyPropertyChanged("TransformComponent");
                }
            }
        }

        public Component RenderComponent
        {
            get { return render; }
            set
            {
                if (render != value)
                {
                    if (null != render)
                    {
                        render.LocalPropertyChanged -= OnPropertyChanged;
                    }

                    render = value;

                    if (null != render)
                    {
                        render.LocalPropertyChanged += OnPropertyChanged;

                        if (render.IsOfType(typeof(RenderComponent)))
                        {
                            Visual = new ShapeEntityVisual(this, render, Entity);
                        }
                        else if (render.IsOfType(typeof(TextRenderComponent)))
                        {
                            Visual = new TextEntityVisual(this, render, Entity);
                        }
                        else if (render.IsOfType(typeof(ImageRenderComponent)))
                        {
                            Visual = new SpriteEntityVisual(this, render, Entity);
                        }
                    }

                    NotifyPropertyChanged("RenderComponent");
                }
            }
        }

        public EntityVisual Visual
        {
            get { return visual; }
            set
            {
                if (visual != value)
                {
                    visual = value;
                    NotifyPropertyChanged("Visual");
                }
            }
        }

        public EntityPresenter(Entity entity) : base(entity)
        {
            Entity.Components.CollectionChanged += OnComponentsChanged;

            UpdateComponents();
        }

        private void OnPropertyChanged(Component component, PluginProperty property)
        {
            NotifyPropertyChanged(property.Name);
        }

        private void OnComponentsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateComponents();
        }

        private void UpdateComponents()
        {
            var currentTransform = Entity.GetComponentByType(typeof(TransformComponent));
            if (currentTransform != TransformComponent)
            {
                TransformComponent = currentTransform;
            }

            var curentRender = Entity.GetComponentByRole(typeof(IRender));
            if (curentRender != RenderComponent)
            {
                RenderComponent = curentRender;
            }
            
            if (null == Visual)
            {
                Visual = new DefaultEntityVisual(this, null, Entity);
            }
        }
    }
}
