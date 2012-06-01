using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Kinectitude.Core.Base;
using SlimDX;
using SlimDX.Direct2D;
using Bitmap = SlimDX.Direct2D.Bitmap;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace Kinectitude.Render
{
    public class RenderService : Service, IDisposable
    {
        public static Color4 ColorFromString(string color)
        {
            Color convertedColor = (Color)ColorConverter.ConvertFromString(color);
            return new Color4((float)convertedColor.R / 255.0f, (float)convertedColor.G / 255.0f, (float)convertedColor.B / 255.0f);

        }

        private RenderTarget renderTarget;
        private SlimDX.Direct2D.Factory drawFactory;
        private Action<RenderTarget> renderAction;
        private readonly Dictionary<Color4, SolidColorBrush> brushes;
        private readonly Dictionary<string, Bitmap> bitmaps;
        private readonly SlimDX.DirectWrite.Factory writeFactory;
        private readonly Color4 clearColor;
        private readonly BitmapProperties bitmapProperties;

        public RenderTarget RenderTarget
        {
            get { return renderTarget; }
            set
            {
                if (null == renderTarget)
                {
                    renderTarget = value;
                    drawFactory = renderTarget.Factory;
                }
            }
        }

        public SlimDX.DirectWrite.Factory DirectWriteFactory
        {
            get { return writeFactory; }
        }

        public SlimDX.Direct2D.Factory Direct2DFactory
        {
            get { return drawFactory; }
        }

        public Action<RenderTarget> RenderAction
        {
            get { return renderAction; }
            set { renderAction = value; }
        }

        public RenderService()
        {
            bitmaps = new Dictionary<string, Bitmap>();
            brushes = new Dictionary<Color4, SolidColorBrush>();
            writeFactory = new SlimDX.DirectWrite.Factory(SlimDX.DirectWrite.FactoryType.Shared);
            clearColor = new Color4(0.30f, 0.30f, 0.80f);
            PixelFormat pixelFormat = new PixelFormat(SlimDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied);
            bitmapProperties = new BitmapProperties() { PixelFormat = pixelFormat };
        }

        public override void OnStart() { }

        public override void OnStop() { }

        public override bool AutoStart()
        {
            return false;
        }

        public SolidColorBrush GetSolidColorBrush(Color4 color, float opacity)
        {
            SolidColorBrush brush;
            brushes.TryGetValue(color, out brush);
            if (null == brush)
            {
                brush = new SolidColorBrush(renderTarget, color, new BrushProperties() { Opacity = opacity });
                brushes[color] = brush;
            }
            return brush;
        }

        public Bitmap GetBitmap(string image)
        {
            Bitmap bitmap;
            bitmaps.TryGetValue(image, out bitmap);
            if (null == bitmap)
            {
                using (System.Drawing.Bitmap source = new System.Drawing.Bitmap(Path.Combine("Assets", image)))
                {
                    System.Drawing.Imaging.BitmapData sourceData = source.LockBits(
                        new Rectangle(0, 0, source.Width, source.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb
                    );

                    using (DataStream dataStream = new DataStream(sourceData.Scan0, sourceData.Stride * sourceData.Height, true, false))
                    {
                        bitmap = new Bitmap(renderTarget, new Size(source.Width, source.Height), dataStream, sourceData.Stride, bitmapProperties);
                        bitmaps[image] = bitmap;
                    }

                    source.UnlockBits(sourceData);
                }
            }
            return bitmap;
        }

        public void Render()
        {
            renderTarget.BeginDraw();
            renderTarget.Transform = Matrix3x2.Identity;
            renderTarget.Clear(clearColor);

            if (null != renderAction)
            {
                renderAction(renderTarget);
            }

            renderTarget.EndDraw();
        }

        public void Dispose()
        {
            renderTarget.Dispose();
            drawFactory.Dispose();

            foreach (SolidColorBrush brush in brushes.Values)
            {
                brush.Dispose();
            }

            foreach (Bitmap bitmap in bitmaps.Values)
            {
                bitmap.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
