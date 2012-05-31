using System;
using Kinectitude.Core.Base;
using SlimDX.Direct2D;
using SlimDX;
using SlimDX.Windows;
using System.Collections.Generic;
using System.Drawing;
using ColorConverter = System.Windows.Media.ColorConverter;
using Color = System.Windows.Media.Color;

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
        private readonly SlimDX.DirectWrite.Factory writeFactory;
        private readonly Color4 clearColor;

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
            brushes = new Dictionary<Color4, SolidColorBrush>();
            writeFactory = new SlimDX.DirectWrite.Factory(SlimDX.DirectWrite.FactoryType.Shared);
            clearColor = new Color4(0.30f, 0.30f, 0.80f);
        }

        public override void OnStart() { }

        public override void OnStop() { }

        public override bool AutoStart()
        {
            return false;
        }

        public SolidColorBrush CreateSolidColorBrush(Color4 color)
        {
            SolidColorBrush brush;
            brushes.TryGetValue(color, out brush);
            if (null == brush)
            {
                brush = new SolidColorBrush(renderTarget, color);
                brushes[color] = brush;
            }
            return brush;
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
        }
    }
}
