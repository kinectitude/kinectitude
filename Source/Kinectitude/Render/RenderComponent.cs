using System;
using Kinectitude.Core;
using Kinectitude.Attributes;
using SlimDX;
using SlimDX.Direct2D;
using System.Drawing;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace Kinectitude.Render
{
    [Plugin("Render Component", "")]
    public class RenderComponent : Component, IRender
    {
        private readonly double width;
        private readonly double height;
        private readonly bool circular;
        private Color4 renderColor;

        [Plugin("Fill Color", "")]
        public string Fillcolor
        {
            set 
            {
                System.Windows.Media.Color color = (System.Windows.Media.Color)ColorConverter.ConvertFromString(value);
                renderColor = new Color4((float)color.R / 255.0f, (float)color.G / 255.0f, (float)color.B / 255.0f);
            } 
        }

        private double X
        {
            get { return double.Parse(Entity["x"]); }
        }

        private double Y
        {
            get { return double.Parse(Entity["y"]); }
        }

        public RenderComponent(Entity entity) : base(entity)
        {
            if (null != Entity["radius"])
            {
                double radius = 2 * double.Parse(Entity["radius"]);
                width = radius;
                height = radius;
                circular = true;
            }
            else
            {
                width = double.Parse(Entity["width"]);
                height = double.Parse(Entity["height"]);
                circular = false;
            }
        }

        public override Type ManagerType()
        {
            return typeof(RenderManager);
        }

        public void Initialize(RenderManager manager) { }

        public void Render(SlimDX.Direct2D.RenderTarget renderTarget)
        {
            SolidColorBrush brush = new SolidColorBrush(renderTarget, renderColor);
            if (circular)
            {
                Ellipse ellipse = new Ellipse()
                {
                    Center = new System.Drawing.PointF((float)X, (float)Y),
                    RadiusX = (float)width / 2.0f,
                    RadiusY = (float)height / 2.0f
                };
                renderTarget.FillEllipse(brush, ellipse);
            }
            else
            {
                RectangleF rectangle = new RectangleF((float)X, (float)Y, (float)width, (float)height);
                renderTarget.FillRectangle(brush, rectangle);
            }
        }
    }
}