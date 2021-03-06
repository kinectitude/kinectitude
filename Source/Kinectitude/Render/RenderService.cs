//-----------------------------------------------------------------------
// <copyright file="RenderService.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

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
using Kinectitude.Core.Attributes;

namespace Kinectitude.Render
{
    [Plugin("Render Service", "")]
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
        private readonly Dictionary<Tuple<Color4, float>, SolidColorBrush> brushes;
        private readonly Dictionary<string, Bitmap> bitmaps;
        private readonly SlimDX.DirectWrite.Factory writeFactory;
        private readonly Color4 clearColor;
        private readonly BitmapProperties bitmapProperties;

        [PluginProperty("Window Width", "", 800)]
        public float Width { get; set; }

        [PluginProperty("Window Height", "", 600)]
        public float Height { get; set; }

        public SizeF Dpi { get; set; }

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
            brushes = new Dictionary<Tuple<Color4, float>, SolidColorBrush>();
            writeFactory = new SlimDX.DirectWrite.Factory(SlimDX.DirectWrite.FactoryType.Shared);
            clearColor = new Color4(1, 1, 1);
            PixelFormat pixelFormat = new PixelFormat(SlimDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied);
            bitmapProperties = new BitmapProperties() { PixelFormat = pixelFormat };
        }

        public override void OnStart() { }

        public override void OnStop() { }

        public override bool AutoStart()
        {
            return false;
        }

        public PointF ConvertPixelsToDips(PointF pixels)
        {
            return new PointF(pixels.X * 96.0f / Dpi.Width, pixels.Y * 96.0f / Dpi.Height);
        }

        public PointF ConvertDipsToPixels(PointF dips)
        {
            return new PointF(dips.X * Dpi.Width / 96.0f, dips.Y * Dpi.Height / 96.0f);
        }

        public SolidColorBrush GetSolidColorBrush(Color4 color, float opacity)
        {
            var key = new Tuple<Color4, float>(color, opacity);

            SolidColorBrush brush;
            brushes.TryGetValue(key, out brush);
            if (null == brush)
            {
                brush = new SolidColorBrush(renderTarget, color, new BrushProperties() { Opacity = opacity });
                brushes[key] = brush;
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
                        System.Drawing.Imaging.PixelFormat.Format32bppPArgb
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
