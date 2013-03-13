using Kinectitude.Editor.Models;
using Kinectitude.Render;
using System;
using System.Windows.Media;

namespace Kinectitude.Editor.Views.Scenes.Presenters
{
    internal sealed class ShapeEntityVisual : EntityVisual
    {
        public RenderComponent.ShapeType Shape
        {
            get { return GetEnumValue<RenderComponent, RenderComponent.ShapeType>("Shape"); }
            set { SetValue<RenderComponent, RenderComponent.ShapeType>("Shape", value); }
        }

        public Brush FillColor
        {
            get
            {
                try
                {
                    return (Brush)BrushConverter.ConvertFromString(GetStringValue<RenderComponent>("FillColor"));
                }
                catch (Exception)
                {
                    return Brushes.Black;
                }
            }
        }

        public float LineThickness
        {
            get { return GetFloatValue<RenderComponent>("LineThickness"); }
        }

        public Brush LineColor
        {
            get
            {
                try
                {
                    return (Brush)BrushConverter.ConvertFromString(GetStringValue<RenderComponent>("LineColor"));
                }
                catch (Exception)
                {
                    return Brushes.Black;
                }
            }
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
