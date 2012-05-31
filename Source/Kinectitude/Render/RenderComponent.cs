using System.Drawing;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using SlimDX;
using SlimDX.Direct2D;
using RenderTarget = SlimDX.Direct2D.RenderTarget;

namespace Kinectitude.Render
{
    [Plugin("Render Component", "")]
    public class RenderComponent : Component, IRender
    {
        public enum ShapeType
        {
            Rectangle,
            Ellipse
        }

        private Color4 renderColor;
        private SolidColorBrush brush;
        private TransformComponent transformComponent;
        private RenderManager renderManager;
        private Ellipse ellipse;
        private RectangleF rectangle;

        [Plugin("Shape", "")]
        public ShapeType Shape
        {
            get;
            set;
        }

        [Plugin("Fill Color", "")]
        public string FillColor
        {
            set { renderColor = RenderService.ColorFromString(value); } 
        }

        public RenderComponent() { }

        public void Render(RenderTarget renderTarget)
        {
            if (Shape == ShapeType.Ellipse)
            {
                renderTarget.FillEllipse(brush, ellipse);
            }
            else if (Shape == ShapeType.Rectangle)
            {
                renderTarget.FillRectangle(brush, rectangle);
            }
        }

        public override void Ready()
        {
            renderManager = GetManager<RenderManager>();
            renderManager.Add(this);

            brush = renderManager.CreateSolidColorBrush(renderColor);

            transformComponent = GetComponent<TransformComponent>();
            transformComponent.SubscribeToX(this, OnTransformChanged);
            transformComponent.SubscribeToY(this, OnTransformChanged);
            transformComponent.SubscribeToWidth(this, OnTransformChanged);
            transformComponent.SubscribeToHeight(this, OnTransformChanged);

            if (Shape == ShapeType.Ellipse)
            {
                ellipse = new Ellipse()
                {
                    Center = new PointF(transformComponent.X, transformComponent.Y),
                    RadiusX = transformComponent.Width / 2.0f,
                    RadiusY = transformComponent.Height / 2.0f
                };
            }
            else if (Shape == ShapeType.Rectangle)
            {
                rectangle = new RectangleF()
                {
                    X = transformComponent.X - transformComponent.Width / 2.0f,
                    Y = transformComponent.Y - transformComponent.Height / 2.0f,
                    Width = transformComponent.Width,
                    Height = transformComponent.Height
                };
            }
        }

        public void OnTransformChanged()
        {
            if (Shape == ShapeType.Ellipse)
            {
                ellipse.Center = new PointF(transformComponent.X, transformComponent.Y);
                ellipse.RadiusX = transformComponent.Width / 2.0f;
                ellipse.RadiusY = transformComponent.Height / 2.0f;
            }
            else if (Shape == ShapeType.Rectangle)
            {
                rectangle.X = transformComponent.X - transformComponent.Width / 2.0f;
                rectangle.Y = transformComponent.Y - transformComponent.Height / 2.0f;
                rectangle.Width = transformComponent.Width;
                rectangle.Height = transformComponent.Height;
            }
        }

        public override void Destroy()
        {
            renderManager.Remove(this);
        }
    }
}