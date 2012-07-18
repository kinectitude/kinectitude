using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;

namespace Kinectitude.Core.Actions
{
    [Plugin("Set position", "")]
    internal sealed class SetPositionAction : Action
    {
        [Plugin("X", "The new X position for the Entity")]
        public float X { get; set; }

        [Plugin("Y", "The new Y position for the Entity")]
        public float Y { get; set; }

        public SetPositionAction() { }

        public override void Run()
        {
            TransformComponent tc = GetComponent<TransformComponent>();
            tc.X = X;
            tc.Y = Y;
        }
    }
}
