using Kinectitude.Core.Components;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.Models.Values;
using Kinectitude.Render;
using System.Collections.Specialized;
using System.Windows.Input;

namespace Kinectitude.Editor.Views.Scenes.Presenters
{
    internal sealed class Translation
    {
        public EntityPresenter Presenter { get; private set; }
        public double StartX { get; private set; }
        public double StartY { get; private set; }
        public double EndX { get; private set; }
        public double EndY { get; private set; }

        public Translation(EntityPresenter presenter, double startX, double startY, double endX, double endY)
        {
            Presenter = presenter;
            StartX = startX;
            StartY = startY;
            EndX = endX;
            EndY = endY;
        }
    }

    internal sealed class Location
    {
        public EntityPresenter Presenter { get; private set; }
        public double StartX { get; private set; }
        public double StartY { get; private set; }
        public double ObservedWidth { get; private set; }
        public double ObservedHeight { get; private set; }

        public Location(EntityPresenter presenter, double startX, double startY, double observedWidth, double observedHeight)
        {
            Presenter = presenter;
            StartX = startX;
            StartY = startY;
            ObservedWidth = observedWidth;
            ObservedHeight = observedHeight;
        }
    }

    internal sealed class Depth
    {
        public EntityPresenter Presenter { get; private set; }
        public int StartIndex { get; private set; }

        public Depth(EntityPresenter presenter, int startIndex)
        {
            Presenter = presenter;
            StartIndex = startIndex;
        }
    }

    internal sealed class EntityPresenter : EntityBase
    {
        private EntityVisual visual;
        private Component transform;
        private Component render;
        private double startX;
        private double startY;
        private double displayX;
        private double displayY;
        private double observedWidth;
        private double observedHeight;
        private bool selected;

        public double StartX
        {
            get { return startX; }
            set
            {
                if (startX != value)
                {
                    startX = value;
                    NotifyPropertyChanged("StartX");
                }
            }
        }

        public double StartY
        {
            get { return startY; }
            set
            {
                if (startY != value)
                {
                    startY = value;
                    NotifyPropertyChanged("StartY");
                }
            }
        }

        [DependsOn("TransformComponent")]
        public double DisplayX
        {
            get { return displayX; }
            set
            {
                if (displayX != value)
                {
                    displayX = value;
                    NotifyPropertyChanged("DisplayX");
                }
            }
        }

        [DependsOn("TransformComponent")]
        public double DisplayY
        {
            get { return displayY; }
            set
            {
                if (displayY != value)
                {
                    displayY = value;
                    NotifyPropertyChanged("DisplayY");
                }
            }
        }

        [DependsOn("TransformComponent"), DependsOn("Width")]
        public double X
        {
            get { return GetDoubleValue<TransformComponent>("X") - Width / 2.0d; }
            set { SetValue<TransformComponent, double>("X", value + Width / 2.0d); }
        }

        [DependsOn("TransformComponent"), DependsOn("Height")]
        public double Y
        {
            get { return GetDoubleValue<TransformComponent>("Y") - Height / 2.0d; }
            set { SetValue<TransformComponent, double>("Y", value + Height / 2.0d); }
        }

        [DependsOn("TransformComponent")]
        public double Width
        {
            get { return GetDoubleValue<TransformComponent>("Width"); }
            set { SetValue<TransformComponent, double>("Width", value); }
        }

        [DependsOn("TransformComponent")]
        public double Height
        {
            get { return GetDoubleValue<TransformComponent>("Height"); }
            set { SetValue<TransformComponent, double>("Height", value); }
        }

        public double ObservedWidth
        {
            get { return observedWidth; }
            set
            {
                if (observedWidth != value)
                {
                    observedWidth = value;
                    NotifyPropertyChanged("ObservedWidth");
                }
            }
        }

        public double ObservedHeight
        {
            get { return observedHeight; }
            set
            {
                if (observedHeight != value)
                {
                    observedHeight = value;
                    NotifyPropertyChanged("ObservedHeight");
                }
            }
        }

        public bool IsSelected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        [DependsOn("TransformComponent")]
        public double Rotation
        {
            get { return GetDoubleValue<TransformComponent>("Rotation"); }
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
                        return property.GetFloatValue();
                    }
                }

                return 1.0f;
            }
            set
            {
                if (null != RenderComponent)
                {
                    render.SetProperty("Opacity", new Value(value.ToString()));
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

        public ICommand OpenEntityCommand { get; private set; }

        public EntityPresenter(Entity entity) : base(entity)
        {
            Entity.Components.CollectionChanged += OnComponentsChanged;

            DisplayX = X;
            DisplayY = Y;

            UpdateComponents();

            OpenEntityCommand = new DelegateCommand(null, p => Workspace.Instance.Project.OpenItem(Entity));
        }

        private void OnPropertyChanged(Component component, PluginProperty property)
        {
            if (component.Plugin.CoreType == typeof(TransformComponent))
            {
                if (property.Name == "X" || property.Name == "Width")
                {
                    DisplayX = X;
                }
                else if (property.Name == "Y" || property.Name == "Height")
                {
                    DisplayY = Y;
                }
            }
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

        public Translation GetTranslation()
        {
            return new Translation(this, StartX, StartY, DisplayX, DisplayY);
        }

        public Location GetLocation()
        {
            return new Location(this, DisplayX, DisplayY, ObservedWidth, ObservedHeight);
        }

        public Depth GetDepth()
        {
            return new Depth(this, Entity.Index);
        }
    }
}
