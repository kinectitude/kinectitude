using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Factory = SlimDX.DirectWrite.Factory;
using SlimDX.Direct2D;
using Kinectitude.Attributes;

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
