using Kinectitude.Attributes;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Actions
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
