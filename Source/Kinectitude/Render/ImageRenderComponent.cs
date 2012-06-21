using System.Drawing;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using Bitmap = SlimDX.Direct2D.Bitmap;
using RenderTarget = SlimDX.Direct2D.RenderTarget;

namespace Kinectitude.Render
{
    public class ImageRenderComponent : Component, IRender
    {
        private RenderManager renderManager;
        private TransformComponent transformComponent;
        private Bitmap bitmap;
        private RectangleF rectangle;

        [Plugin("Image", "")]
        public string Image
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

        public ImageRenderComponent()
        {
            Opacity = 1.0f;
        }

        public override void Ready()
        {
            renderManager = GetManager<RenderManager>();
            renderManager.Add(this);

            transformComponent = GetComponent<TransformComponent>();
            rectangle = new RectangleF();
            bitmap = renderManager.GetBitmap(Image);
        }

        public void UpdateTransform()
        {
            rectangle.X = transformComponent.X - transformComponent.Width / 2.0f;
            rectangle.Y = transformComponent.Y - transformComponent.Height / 2.0f;
            rectangle.Width = transformComponent.Width;
            rectangle.Height = transformComponent.Height;
        }

        public override void Destroy()
        {
            renderManager.Remove(this);
        }

        public void Render(RenderTarget renderTarget)
        {
            UpdateTransform();
            renderTarget.DrawBitmap(bitmap, rectangle, Opacity);
        }
    }
}
