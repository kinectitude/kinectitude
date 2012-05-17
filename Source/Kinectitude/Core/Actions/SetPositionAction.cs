using Kinectitude.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using Action = Kinectitude.Core.Base.Action;

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
