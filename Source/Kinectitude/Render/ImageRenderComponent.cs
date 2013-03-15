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
        [PluginProperty("Image", "", null, 
                        "Image files|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico", 
                        "Select an image file")]
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
        [PluginProperty("Animated", "")]
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
        [PluginProperty("Row", "")]
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
        [PluginProperty("Duration", "")]
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

        private bool stretched;
        [PluginProperty("Stretch", "", true)]
        public bool Stretched
        {
            get { return stretched; }
            set
            {
                if (stretched != value)
                {
                    stretched = value;
                    Change("Stretched");
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
            totalFrames = bitmap.PixelSize.Width / (int)transformComponent.Width;
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

            if (Stretched)
            {
                sourceRectangle.Width = bitmap.PixelSize.Width / scaleX;
                sourceRectangle.Height = bitmap.PixelSize.Height / scaleY;
            }
            else
            {
                sourceRectangle.Width = transformComponent.Height / scaleX;
                sourceRectangle.Height = transformComponent.Width / scaleY;
            }

            renderTarget.DrawBitmap(bitmap, destRectangle, Opacity, SlimDX.Direct2D.InterpolationMode.Linear, sourceRectangle);
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
