using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;

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
            Game game = Event.Entity.Scene.Game;
            game.PushScene(Target);
        }
    }
}
