using System;
using System.Collections.Generic;
using System.Drawing;
using Kinectitude.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using Kinectitude.Core.Exceptions;
using SlimDX;
using SlimDX.Direct2D;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace Kinectitude.Render
{
    [Plugin("Render Component", "")]
    public class RenderComponent : Component, IRender
    {
        private bool circular;
        private Color4 renderColor;

        private TransformComponent tc;

        [Plugin("Shape", "")]
        public string Shape { get; set; }

        [Plugin("Fill Color", "")]
        public string FillColor
        {
            set 
            {
                System.Windows.Media.Color color = (System.Windows.Media.Color)ColorConverter.ConvertFromString(value);
                renderColor = new Color4((float)color.R / 255.0f, (float)color.G / 255.0f, (float)color.B / 255.0f);
            } 
        }

        public RenderComponent() : base() { }

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
                    Center = new System.Drawing.PointF(tc.X, tc.Y),
                    RadiusX = tc.Width,
                    RadiusY = tc.Height
                };
                renderTarget.FillEllipse(brush, ellipse);
            }
            else
            {
                RectangleF rectangle =
                    new RectangleF(tc.X - tc.Width / 2.0f, tc.Y - tc.Height / 2.0f, tc.Width, tc.Height);
                renderTarget.FillRectangle(brush, rectangle);
            }
        }

        public override void Ready()
        {
            List<Type> missing = new List<Type>();
            tc = Entity.GetComponent(typeof(TransformComponent)) as TransformComponent;
            if (null == tc)
            {
                missing.Add(typeof(TransformComponent));
                throw MissingRequirementsException.MissingRequirement(this, missing);
            }
            circular = "circle" == Shape;
        }
    }
}