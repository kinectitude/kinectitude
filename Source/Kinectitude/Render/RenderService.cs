using System;
using Kinectitude.Core.Base;
using SlimDX.Direct2D;
using Factory = SlimDX.DirectWrite.Factory;

namespace Kinectitude.Render
{
    public class RenderService : Service
    {
        public Factory Factory{get; set;}
        public Action<RenderTarget> RenderTargetAction {get; internal set;}

        public RenderService() { }

        public override void OnStart() { }

        public override void OnStop() { }

        public override bool AutoStart()
        {
            return false;
        }
    }
}
