using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Kinectitude.Core.Base;
using Kinectitude.Core.Loaders;
using Kinectitude.Render;
using SlimDX.Direct2D;
using SlimDX.Windows;
using Factory = SlimDX.Direct2D.Factory;

namespace Kinectitude.Player
{
    internal sealed class Application : IDisposable
    {
        private const float TimeStep = 1.0f / 60.0f;

        private readonly RenderService renderService;
        private readonly RenderForm form;
        private readonly Game game;

        public Application()
        {
            Assembly loaded = Assembly.GetAssembly(typeof(RenderService));
            GameLoader gameLoader = GameLoader.GetGameLoader("game.xml", new Assembly[] { loaded });
            game = gameLoader.Game;

            Factory drawFactory = new SlimDX.Direct2D.Factory();
            SizeF dpi = drawFactory.DesktopDpi;

            Size size = new Size((int)(game.Width * dpi.Width / 96.0f), (int)(game.Height * dpi.Height / 96.0f));

            form = new RenderForm(game.Name)
            {
                ClientSize = size,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                MaximizeBox = false
            };
            
            renderService = game.GetService<RenderService>();
            renderService.RenderTarget = new WindowRenderTarget(drawFactory, new WindowRenderTargetProperties()
            {
                Handle = form.Handle,
                PixelSize = size
            });
        }

        public void Dispose()
        {
            renderService.Dispose();
            form.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Run()
        {
            Clock clock = new Clock();
            float accumulator = 0.0f;

            clock.Start();
            game.Start();

            MessagePump.Run(form, () =>
                {
                    float frameDelta = clock.Update();
                    accumulator += frameDelta;

                    while (accumulator > TimeStep)
                    {
                        game.OnUpdate(TimeStep);
                        accumulator -= TimeStep;
                    }

                    renderService.Render();
                }
            );
        }
    }
}
