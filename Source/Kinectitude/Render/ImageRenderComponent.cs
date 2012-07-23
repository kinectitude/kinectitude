using System.Drawing;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using Bitmap = SlimDX.Direct2D.Bitmap;
using RenderTarget = SlimDX.Direct2D.RenderTarget;
using Kinectitude.Core.ComponentInterfaces;

namespace Kinectitude.Render
{
    [Plugin("Image Render Component", "")]
    public class ImageRenderComponent : BaseRenderComponent
    {
        private Bitmap bitmap;
        private RectangleF destRectangle;
        private RectangleF sourceRectangle;
        private int currentFrame;
        private int totalFrames;
        private float frameTime;
        private float scaleX;
        private float scaleY;

        private string image;
        [Plugin("Image", "")]
        public string Image
        {
            get { return image; }
            set
            {
                if (image != value)
                {
                    image = value;

                    if (null != renderManager)
                    {
                        bitmap = renderManager.GetBitmap(image);
                    }

                    Change("Image");
                }
            }
        }

        private bool animated;
        [Plugin("Animated", "")]
        public bool Animated
        {
            get { return animated; }
            set
            {
                if (animated != value)
                {
                    animated = value;
                    Change("Animated");
                }
            }
        }

        private int row;
        [Plugin("Row", "")]
        public int Row
        {
            get { return row; }
            set
            {
                if (row != value)
                {
                    row = value;
                    Change("Row");
                }
            }
        }

        private float duration;
        [Plugin("Duration", "")]
        public float Duration
        {
            get { return duration; }
            set
            {
                if (duration != value)
                {
                    duration = value;
                    Change("Duration");
                }
            }
        }

        public ImageRenderComponent()
        {
            Animated = false;
            currentFrame = 0;

            destRectangle = new RectangleF();
            sourceRectangle = new RectangleF();
        }

        protected override void OnReady()
        {
            Row = 1;
            bitmap = renderManager.GetBitmap(Image);
            totalFrames = bitmap.PixelSize.Width / transformComponent.Width;
            scaleX = bitmap.DotsPerInch.Width / 96.0f;
            scaleY = bitmap.DotsPerInch.Height / 96.0f;
        }

        protected override void OnRender(RenderTarget renderTarget)
        {
            destRectangle.X = transformComponent.X - transformComponent.Width / 2.0f;
            destRectangle.Y = transformComponent.Y - transformComponent.Height / 2.0f;
            destRectangle.Width = transformComponent.Width;
            destRectangle.Height = transformComponent.Height;

            sourceRectangle.X = transformComponent.Width * currentFrame / scaleX;
            sourceRectangle.Y = transformComponent.Height * (Row - 1) / scaleY;
            sourceRectangle.Width = transformComponent.Width / scaleX;
            sourceRectangle.Height = transformComponent.Height / scaleY;

            renderTarget.DrawBitmap(bitmap, destRectangle, Opacity, SlimDX.Direct2D.InterpolationMode.Linear, sourceRectangle);

            //if (Animated)
            //{
            //    currentFrame = (currentFrame + 1) % totalFrames;
            //}
        }

        public override void OnUpdate(float frameDelta)
        {
            if (Animated)
            {
                frameTime += frameDelta;
                if (frameTime > Duration / totalFrames)
                {
                    frameTime = 0.0f;
                    currentFrame = (currentFrame + 1) % totalFrames;
                }
            }
        }
    }
}
