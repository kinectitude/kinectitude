using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    [Plugin("Push scene {Target}", "")]
    public sealed class PushSceneAction : Action
    {
        [Plugin("Scene", "")]
        public IExpressionReader Target { get; set; }

        public PushSceneAction() { }

        public override void Run()
        {
            Game game = Event.Entity.Scene.Game;
            game.PushScene(Target.GetValue());
        }
    }
}
