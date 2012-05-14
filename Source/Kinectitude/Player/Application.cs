using System;
using SlimDX.Windows;
using System.Drawing;
using System.Windows.Forms;
using SlimDX.Direct2D;
using SlimDX;
using SlimDX.DirectWrite;
using FontStyle = SlimDX.DirectWrite.FontStyle;
using Kinectitude.Core;
using Kinectitude.Core.Base;
using Kinectitude.Core.Loaders;
using System.Diagnostics;

namespace Kinectitude.Player
{
    internal sealed class Application : IDisposable
    {
        private const float TimeStep = 1.0f / 60.0f;

        private readonly Game game;
        private readonly RenderForm form;
        private readonly WindowRenderTarget renderTarget;
        private readonly SlimDX.Direct2D.Factory drawFactory;
        private readonly SlimDX.DirectWrite.Factory writeFactory;
        private readonly SolidColorBrush textBrush;
        private readonly TextFormat textFormat;
        private readonly Color4 clearColor;
        private readonly Clock clock;

        private float frameDelta;
        private float frameAccumulator;
        private float framesPerSecond;
        private int frameCount;
        private float accumulator;

        public Application()
        {
            drawFactory = new SlimDX.Direct2D.Factory();
            SizeF dpi = drawFactory.DesktopDpi;

            GameLoader gameLoader = GameLoader.GetGameLoader("game.xml");
            game = gameLoader.Game;

            Size size = new Size( (int)(game.Width * dpi.Width / 96.0f), (int)(game.Height * dpi.Height / 96.0f) );
            form = new RenderForm(game.Name);
            form.ClientSize = size;
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.MaximizeBox = false;

            renderTarget = new WindowRenderTarget(drawFactory, new WindowRenderTargetProperties()
            {
                Handle = form.Handle,
                PixelSize = size
            });
            
            writeFactory = new SlimDX.DirectWrite.Factory(SlimDX.DirectWrite.FactoryType.Shared);
            textFormat = writeFactory.CreateTextFormat("Consolas", FontWeight.Regular, FontStyle.Normal, FontStretch.Normal, 18.0f, "en-us");
            textFormat.TextAlignment = TextAlignment.Center;
            textFormat.ParagraphAlignment = ParagraphAlignment.Center;
            textBrush = new SolidColorBrush(renderTarget, new Color4(1.0f, 1.0f, 1.0f));
            clearColor = new Color4(0.30f, 0.30f, 0.80f);

            gameLoader.RegisterService(typeof(SlimDX.DirectWrite.Factory), writeFactory);

            clock = new Clock();
            accumulator = 0.0f;
        }

        public void Dispose()
        {
            renderTarget.Dispose();
            drawFactory.Dispose();
            form.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Run()
        {
            clock.Start();
            game.Start();

            MessagePump.Run(form, () =>
                {
                    frameDelta = clock.Update();
                    accumulator += frameDelta;

                    while (accumulator > TimeStep)
                    {
                        Update(TimeStep);
                        accumulator -= TimeStep;
                    }

                    Render();
                }
            );
        }

        private void Update(float frameDelta)
        {
            game.OnUpdate(frameDelta);
        }

        private void Render()
        {
            frameAccumulator += frameDelta;
            ++frameCount;

            if (frameAccumulator >= 1.0f)
            {
                framesPerSecond = frameCount / frameAccumulator;
                frameAccumulator = 0.0f;
                frameCount = 0;
            }

            renderTarget.BeginDraw();
            renderTarget.Transform = Matrix3x2.Identity;
            renderTarget.Clear(clearColor);

            if (null != game.OnRender)
            {
                game.OnRender(renderTarget);
            }

            RectangleF layoutRect = form.ClientRectangle;
            renderTarget.DrawText(framesPerSecond.ToString(), textFormat, layoutRect, textBrush);

            renderTarget.EndDraw();
        }
    }
}
