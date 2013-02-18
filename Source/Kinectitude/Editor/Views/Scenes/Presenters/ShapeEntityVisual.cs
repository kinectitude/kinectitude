using Kinectitude.Editor.Models;
using Kinectitude.Render;

namespace Kinectitude.Editor.Views.Scenes.Presenters
{
    internal sealed class ShapeEntityVisual : EntityVisual
    {
        public RenderComponent.ShapeType Shape
        {
            get { return GetEnumValue<RenderComponent, RenderComponent.ShapeType>("Shape"); }
            set { SetValue<RenderComponent, RenderComponent.ShapeType>("Shape", value); }
        }

        public string FillColor
        {
            get { return GetStringValue<RenderComponent>("FillColor"); }
            set { SetValue<RenderComponent, string>("FillColor", value); }
        }

        public float LineThickness
        {
            get { return GetFloatValue<RenderComponent>("LineThickness"); }
            set { SetValue<RenderComponent, float>("LineThickness", value); }
        }

        public string LineColor
        {
            get { return GetStringValue<RenderComponent>("LineColor"); }
            set { SetValue<RenderComponent, string>("LineColor", value); }
        }

        [DependsOn("Shape")]
        public bool IsElliptical
        {
            get
            {
                return GetEnumValue<RenderComponent, RenderComponent.ShapeType>("Shape") ==
                    Kinectitude.Render.RenderComponent.ShapeType.Ellipse;
            }
        }

        [DependsOn("IsElliptical")]
        public bool IsRectangular
        {
            get { return !IsElliptical; }
        }

        public ShapeEntityVisual(EntityPresenter presenter, Component render, Entity entity) : base(presenter, render, entity) { }
    }
}
