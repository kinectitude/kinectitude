using Kinectitude.Attributes;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Actions
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