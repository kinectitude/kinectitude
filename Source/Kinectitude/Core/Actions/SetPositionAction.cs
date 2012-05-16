using System;
using Kinectitude.Core;
using System.Xml;
using Kinectitude.Attributes;
using Kinectitude.Core.Base;
using Action = Kinectitude.Core.Base.Action;
using Kinectitude.Core.Components;

namespace Kinectitude.Core.Actions
{
    [Plugin("Set position", "")]
    public class SetPositionAction : Action
    {
        [Plugin("X", "")]
        public float X { get; set; }

        [Plugin("Y", "")]
        public float Y { get; set; }

        public SetPositionAction() : base() { }

        public override void Run()
        {
            TransformComponent tc = Event.Entity.GetComponent(typeof(TransformComponent)) as TransformComponent;
            tc.X = X;
            tc.Y = Y;
        }
    }
}
