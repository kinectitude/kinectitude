using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Attributes;

namespace Kinectitude.Core
{
    [Plugin("Scene starts", "")]
    public class SceneStartsEvent : Event
    {
        public SceneStartsEvent() { }

        public override void OnInitialize()
        {
            Scene.OnStart.Add(this);
        }
    }
}