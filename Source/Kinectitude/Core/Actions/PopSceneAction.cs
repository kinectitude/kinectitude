using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Kinectitude.Attributes;

namespace Kinectitude.Core
{
    [Plugin("Pop the scene", "")]
    public sealed class PopSceneAction : Action
    {
        public PopSceneAction() { }

        public override void Run()
        {
            Event.Scene.Game.PopScene();
        }
    }
}
