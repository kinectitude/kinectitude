using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    [Plugin("push scene {Target}", "Push a scene")]
    public sealed class PushSceneAction : Action
    {
        [PluginProperty("Scene", "")]
        public ValueReader Target { get; set; }

        public PushSceneAction() { }

        public override void Run()
        {
            Game game = Event.Entity.Scene.Game;
            game.PushScene(Target);
        }
    }
}
