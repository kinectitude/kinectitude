using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Attributes;

namespace Kinectitude.Core
{
    [Plugin("Fire a trigger", "")]
    internal sealed class FireTriggerAction : Action
    {
        [Plugin("Trigger", "")]
        public string Name { get; set; }

        public FireTriggerAction() { }

        public override void Run()
        {
            Event.Scene.FireTrigger(Name);
        }
    }
}
