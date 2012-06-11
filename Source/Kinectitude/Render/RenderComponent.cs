using System.Drawing;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
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

        private SolidColorBrush fillBrush;
        private SolidColorBrush lineBrush;
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
            get;
            set;
        }

        [Plugin("Line Thickness", "")]
        public float LineThickness
        {
            get;
            set;
        }

        [Plugin("Line Color", "")]
        public string LineColor
        {
            get;
            set;
        }

        [Plugin("Opacity", "")]
        public float Opacity
        {
            get;
            set;
        }

        public RenderComponent()
        {
            Opacity = 1.0f;
            LineColor = "Black";
            LineThickness = 0.0f;
        }

        public void Render(RenderTarget renderTarget)
        {
            if (Shape == ShapeType.Ellipse)
            {
                renderTarget.FillEllipse(fillBrush, ellipse);
                renderTarget.DrawEllipse(lineBrush, ellipse, LineThickness);
            }
            else if (Shape == ShapeType.Rectangle)
            {
                renderTarget.FillRectangle(fillBrush, rectangle);
                renderTarget.DrawRectangle(lineBrush, rectangle, LineThickness);
            }
        }

        public override void Ready()
        {
            renderManager = GetManager<RenderManager>();
            renderManager.Add(this);

            fillBrush = renderManager.GetSolidColorBrush(FillColor, Opacity);
            lineBrush = renderManager.GetSolidColorBrush(LineColor, 1.0f);

            transformComponent = GetComponent<TransformComponent>();
            transformComponent.SubscribeToX(this, UpdateTransform);
            transformComponent.SubscribeToY(this, UpdateTransform);
            transformComponent.SubscribeToWidth(this, UpdateTransform);
            transformComponent.SubscribeToHeight(this, UpdateTransform);

            if (Shape == ShapeType.Ellipse)
            {
                ellipse = new Ellipse();
            }
            else if (Shape == ShapeType.Rectangle)
            {
                rectangle = new RectangleF();
            }

            UpdateTransform();
        }

        public void UpdateTransform()
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