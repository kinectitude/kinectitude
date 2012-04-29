using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Kinectitude.Attributes;

namespace Kinectitude.Core
{
    [Plugin("Push a scene", "")]
    public sealed class PushSceneAction : Action
    {
        [Plugin("Scene", "")]
        public string Target { get; set; }

        public PushSceneAction() { }

        public override void Run()
        {
            Event.Scene.Game.PushScene(Target);
        }
    }
}
