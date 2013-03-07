﻿using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Kinectitude.Core.Base;
using Kinectitude.Core.Loaders;
using Kinectitude.Render;
using SlimDX.Direct2D;
using SlimDX.Windows;
using Factory = SlimDX.Direct2D.Factory;
using Kinectitude.DirectInput;

namespace Kinectitude.Player
{
    internal sealed class Application : IDisposable
    {
        private const float TimeStep = 1.0f / 60.0f;

        private readonly RenderService renderService;
        private readonly RenderForm form;
        private readonly Game game;
        private readonly DirectInputService directInputService;

        private void die(string message)
        {
            MessageBox.Show(message, "Game Error");
            Environment.Exit(1);
        }

        public Application()
        {
            Assembly renderAssembly = Assembly.GetAssembly(typeof(RenderService));
            Assembly directAssembly = Assembly.GetAssembly(typeof(DirectInputService));

            Factory drawFactory = new SlimDX.Direct2D.Factory();
            SizeF dpi = drawFactory.DesktopDpi;

            Func<Tuple<int, int>> windowOffset = () => new Tuple<int, int>(form.Left, form.Top);

            GameLoader gameLoader = new GameLoader("game.kgl", new Assembly[] { renderAssembly, directAssembly },
                96 / dpi.Width, 90 / dpi.Height, windowOffset, die);

            game = gameLoader.CreateGame();
            renderService = game.GetService<RenderService>();
            
            Size size = new Size((int)(renderService.Width * dpi.Width / 96.0f), (int)(renderService.Height * dpi.Height / 96.0f));

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

            directInputService = game.GetService<DirectInputService>();
            directInputService.Control = form;
            int y = (int)((SystemInformation.VirtualScreen.Height - renderService.Height * dpi.Height / 96.0) / 2);
            if (y < 0) y = 0;
            int x = (int)((SystemInformation.VirtualScreen.Width - renderService.Width * dpi.Width / 96.0) / 2);
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
