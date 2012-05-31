﻿using Kinectitude.Core.Base;
using SlimDX.Direct2D;
using SlimDX.DirectWrite;
using SlimDX;

namespace Kinectitude.Render
{    
    public class RenderManager : Manager<IRender>
    {
        private readonly RenderService renderService;

        public SlimDX.DirectWrite.Factory DirectWriteFactory
        {
            get { return renderService.DirectWriteFactory; }
        }

        public RenderManager()
        {
            renderService = GetService<RenderService>();

            SlimDX.DirectWrite.Factory factory = renderService.DirectWriteFactory;
        }

        public void Render(RenderTarget renderTarget)
        {
            foreach (IRender render in Children)
            {
                render.Render(renderTarget);
            }
        }

        protected override void OnStart()
        {
            renderService.RenderAction += Render;
        }

        protected override void OnStop()
        {
            renderService.RenderAction -= Render;
        }

        public SolidColorBrush CreateSolidColorBrush(Color4 color)
        {
            return renderService.CreateSolidColorBrush(color);
        }
    }
}
