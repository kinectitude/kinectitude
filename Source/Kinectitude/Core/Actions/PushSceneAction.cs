using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    [Plugin("Push Scene {Target}", "")]
    public sealed class PushSceneAction : Action
    {
        [Plugin("Scene", "")]
        public ValueReader Target { get; set; }

        public PushSceneAction() { }

        public override void Run()
        {
            Game game = Event.Entity.Scene.Game;
            game.PushScene(Target);
        }
    }
}
