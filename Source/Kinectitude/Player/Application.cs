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
using Kinectitude.Input;

namespace Kinectitude.Player
{
    internal sealed class Application : IDisposable
    {
        private const float TimeStep = 1.0f / 60.0f;

        private readonly RenderService renderService;
        private readonly RenderForm form;
        private readonly Game game;
        private readonly InputService inputService;

        private void die(string message)
        {
            MessageBox.Show(message, "Game Error");
            Environment.Exit(1);
        }

        public Application()
        {
            Assembly renderAssembly = Assembly.GetAssembly(typeof(RenderService));
            Assembly inputAssembly = Assembly.GetAssembly(typeof(InputService));

            Factory drawFactory = new SlimDX.Direct2D.Factory();

            GameLoader gameLoader = new GameLoader("game.kgl", new Assembly[] { renderAssembly, inputAssembly }, die);

            game = gameLoader.CreateGame();
            
            renderService = game.GetService<RenderService>();
            renderService.Dpi = drawFactory.DesktopDpi;

            if (renderService.Width == 0)
            {
                renderService.Width = 800;
            }

            if (renderService.Height == 0)
            {
                renderService.Height = 600;
            }

            PointF pixelSize = renderService.ConvertDipsToPixels(new PointF(renderService.Width, renderService.Height));
            Size size = new Size((int)pixelSize.X, (int)pixelSize.Y);

            form = new RenderForm(game.Name)
            {
                ClientSize = size,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                MaximizeBox = false
            };
            
            renderService.RenderTarget = new WindowRenderTarget(drawFactory, new WindowRenderTargetProperties()
            {
                Handle = form.Handle,
                PixelSize = size
            });

            inputService = game.GetService<InputService>();
            inputService.Form = form;

            //directInputService.Control = form;
            


            int y = (int)((SystemInformation.VirtualScreen.Height - renderService.Height * renderService.Dpi.Height / 96.0) / 2);
            if (y < 0) y = 0;
            int x = (int)((SystemInformation.VirtualScreen.Width - renderService.Width * renderService.Dpi.Width / 96.0) / 2);
            if (x < 0) x = 0;
            form.SetDesktopLocation(x, y);
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
                    if (!game.Running) Environment.Exit(0);
                    accumulator -= TimeStep;
                }

                renderService.Render();
            });
        }
    }
}
