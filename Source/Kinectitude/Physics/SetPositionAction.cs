using System;
using Kinectitude.Core;
using System.Xml;
using Kinectitude.Attributes;

namespace Kinectitude.Physics
{
    [Plugin("Set position", "")]
    public class SetPositionAction : Kinectitude.Core.Action
    {
        [Plugin("X", "")]
        public double X { get; set; }

        [Plugin("Y", "")]
        public double Y { get; set; }

        public SetPositionAction() : base() { }

        public override void Run()
        {
            PhysicsComponent pc = Event.Entity.GetComponent(typeof(PhysicsComponent)) as PhysicsComponent;
            pc.OnSetPositionAction(this);
        }
    }
}
