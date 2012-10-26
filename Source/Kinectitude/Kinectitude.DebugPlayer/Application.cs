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
using Button = System.Windows.Forms.Button;

namespace Kinectitude.Player
{
    internal sealed class Application : IDisposable
    {
        private const float TimeStep = 1.0f / 60.0f;

        private readonly RenderService renderService;
        private readonly RenderForm form;
        private readonly Game game;
        private readonly DirectInputService directInputService;

        public Application()
        {
            Assembly renderAssembly = Assembly.GetAssembly(typeof(RenderService));
            Assembly directAssemdly = Assembly.GetAssembly(typeof(DirectInputService));

            Factory drawFactory = new SlimDX.Direct2D.Factory();
            SizeF dpi = drawFactory.DesktopDpi;

            Func<Tuple<int, int>> windowOffset = new Func<Tuple<int, int>>(() => new Tuple<int, int>(form.Left, form.Top));

            GameLoader gameLoader = new GameLoader("Game.xml",
                new Assembly[] { renderAssembly, directAssemdly }, 96 / dpi.Width, 90 / dpi.Height, windowOffset);

            game = gameLoader.CreateGame();
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

            directInputService = game.GetService<DirectInputService>();
            directInputService.Control = form;
            int y = (int)((SystemInformation.VirtualScreen.Height - game.Height * dpi.Height / 96.0)/ 2);
            if(y < 0) y = 0;
            int x = (int)((SystemInformation.VirtualScreen.Width - game.Width * dpi.Width / 96.0) / 2);
            if (x < 0) x = 0;
            form.SetDesktopLocation(x, y);
            form.Width += 150;
            Button pauseResume = new Button();
            pauseResume.Size = new Size(30, 30);
            pauseResume.Text = "Pause";
            pauseResume.Location = new Point((int)(game.Width * dpi.Width / 96.0) + 10, y);
            form.Container.Add(pauseResume);

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
