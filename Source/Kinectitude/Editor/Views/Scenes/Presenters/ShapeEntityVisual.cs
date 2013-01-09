using Kinectitude.Editor.Models;
using Kinectitude.Render;

namespace Kinectitude.Editor.Views.Scenes.Presenters
{
    internal sealed class ShapeEntityVisual : EntityVisual
    {
        public string Shape
        {
            get { return GetValue<RenderComponent, string>("Shape"); }
            set { SetValue<RenderComponent, string>("Shape", value); }
        }

        public string FillColor
        {
            get { return GetValue<RenderComponent, string>("FillColor"); }
            set { SetValue<RenderComponent, string>("FillColor", value); }
        }

        public float LineThickness
        {
            get { return GetValue<RenderComponent, float>("LineThickness"); }
            set { SetValue<RenderComponent, float>("LineThickness", value); }
        }

        public string LineColor
        {
            get { return GetValue<RenderComponent, string>("LineColor"); }
            set { SetValue<RenderComponent, string>("LineColor", value); }
        }

        [DependsOn("Shape")]
        public bool IsElliptical
        {
            get { return GetValue<RenderComponent, string>("Shape") == "Ellipse"; }
        }

        [DependsOn("IsElliptical")]
        public bool IsRectangular
        {
            get { return !IsElliptical; }
        }

        public ShapeEntityVisual(EntityPresenter presenter, Component render, Entity entity) : base(presenter, render, entity) { }
    }
}
