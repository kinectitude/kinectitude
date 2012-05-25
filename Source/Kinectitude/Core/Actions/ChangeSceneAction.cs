using Kinectitude.Attributes;
using Kinectitude.Core.Base;

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
            Game game = Event.Entity.Scene.Game;
            game.RunScene(Target);
        }
    }
}