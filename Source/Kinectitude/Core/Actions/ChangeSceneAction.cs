using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Kinectitude.Attributes;

namespace Kinectitude.Core
{
    [Plugin("Change the scene", "")]
    internal sealed class ChangeSceneAction : Action
    {
        [Plugin("Scene", "")]
        public string Target { get; set; }

        public ChangeSceneAction() { }

        public override void Run()
        {
            Event.Scene.Game.RunScene(Target);
        }
    }
}