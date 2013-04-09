//-----------------------------------------------------------------------
// <copyright file="RenderComponent.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System.Drawing;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using SlimDX.Direct2D;
using RenderTarget = SlimDX.Direct2D.RenderTarget;
using Kinectitude.Core.ComponentInterfaces;
using SlimDX;

namespace Kinectitude.Render
{
    [Plugin("Render Component", "")]
    public class RenderComponent : BaseRenderComponent
    {
        public enum ShapeType
        {
            Rectangle,
            Ellipse
        }

        private SolidColorBrush fillBrush;
        private SolidColorBrush lineBrush;
        private Ellipse ellipse;
        private RectangleF rectangle;
        private ShapeType shape;
        [PluginProperty("Shape", "")]
        public ShapeType Shape
        {
            get { return shape; }
            set
            {
                if (shape != value)
                {
                    shape = value;
                    Change("Shape");
                }
            }
        }

        private string fillColor;
        [PluginProperty("Fill Color", "")]
        public string FillColor
        {
            get { return fillColor; }
            set
            {
                if (fillColor != value)
                {
                    fillColor = value;
                    Change("FillColor");
                }
            }
        }

        private float lineThickness;
        [PluginProperty("Line Thickness", "", 0.0f)]
        public float LineThickness
        {
            get { return lineThickness; }
            set
            {
                if (lineThickness != value)
                {
                    lineThickness = value;
                    Change("LineThickness");
                }
            }
        }

        private string linecolor;
        [PluginProperty("Line Color", "", "Black")]
        public string LineColor
        {
            get { return linecolor; }
            set
            {
                if (linecolor != value)
                {
                    linecolor = value;
                    Change("LineColor");
                }
            }
        }

        public RenderComponent() { }

        protected override void OnRender(RenderTarget renderTarget)
        {
            fillBrush = renderManager.GetSolidColorBrush(FillColor, Opacity);
            lineBrush = renderManager.GetSolidColorBrush(LineColor, Opacity);

            if (Shape == ShapeType.Ellipse)
            {
                ellipse.Center = new PointF(transformComponent.X, transformComponent.Y);
                ellipse.RadiusX = transformComponent.Width / 2.0f;
                ellipse.RadiusY = transformComponent.Height / 2.0f;

                renderTarget.FillEllipse(fillBrush, ellipse);
                renderTarget.DrawEllipse(lineBrush, ellipse, LineThickness);
            }
            else if (Shape == ShapeType.Rectangle)
            {
                rectangle.X = transformComponent.X - transformComponent.Width / 2.0f;
                rectangle.Y = transformComponent.Y - transformComponent.Height / 2.0f;
                rectangle.Width = transformComponent.Width;
                rectangle.Height = transformComponent.Height;

                renderTarget.FillRectangle(fillBrush, rectangle);
                renderTarget.DrawRectangle(lineBrush, rectangle, LineThickness);
            }
        }

        protected override void OnReady()
        {
            if (Shape == ShapeType.Ellipse)
            {
                ellipse = new Ellipse();
            }
            else if (Shape == ShapeType.Rectangle)
            {
                rectangle = new RectangleF();
            }
        }
    }
}
